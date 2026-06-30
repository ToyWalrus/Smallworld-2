using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smallworld.Hooks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityModels;
using SMPlayer = Smallworld.Models.Player;
using SMRacePower = Smallworld.Models.RacePower;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    public Game game;

    private event Action<SMRacePower> OnRacePowerClicked;
    private event Action<SMPlayer> OnPlayerClicked;
    private UIDocument _doc;

    public VisualTreeAsset racePowerButton;
    private List<RacePowerButtonController> racePowerButtons = new();
    private List<PlayerButtonController> playerButtons = new();

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        InitGameFlowButtons();
        InitUIHooks();
    }

    public void SetRacePowerButtonsInteractable(bool interactable)
    {
        var container = _doc.rootVisualElement.Q<VisualElement>("RPContainer");
        container.SetEnabled(interactable);
        if (interactable)
        {
            container.RemoveFromClassList("RPHidden");
        }
        else
        {
            container.AddToClassList("RPHidden");
        }
    }

    public void InitRacePowerButtons(List<SMRacePower> racePowers)
    {
        var area = _doc.rootVisualElement.Q<VisualElement>("RPContainer");
        var tokenSprites = Resources.LoadAll<Sprite>("racetokens").ToList();
        foreach (var rp in racePowers)
        {
            var el = racePowerButton.Instantiate();
            var controller = new RacePowerButtonController(el, tokenSprites);
            controller.SetButtonRacePower(rp);

            area.Add(el);
            racePowerButtons.Add(controller);
            controller.OnClicked += (rp) => OnRacePowerClicked?.Invoke(rp);
        }
    }

    public void InitPlayerButtons(List<Player> players)
    {
        var container = _doc.rootVisualElement.Q<VisualElement>("PlayerContainer");

        for (int i = 0; i < players.Count; ++i)
        {
            var item = container.Q<VisualElement>($"P{i + 1}");
            if (item == null) continue;

            var controller = new PlayerButtonController(item, players[i]);
            playerButtons.Add(controller);
            controller.OnClicked += (player) => OnPlayerClicked?.Invoke(player);
        }

        var extraCount = container.childCount - players.Count;
        while (extraCount > 0)
        {
            var item = container.Q<VisualElement>($"P{4 - extraCount + 1}");
            item.style.display = DisplayStyle.None;
            extraCount--;
        }
    }

    public void ReplaceRacePowerButton(int buttonIndex, SMRacePower newRacePower)
    {
        if (buttonIndex >= racePowerButtons.Count)
        {
            Debug.LogError($"Index {buttonIndex} is out of range");
            return;
        }

        var controller = racePowerButtons[buttonIndex];
        controller.SetButtonRacePower(newRacePower);
    }

    public void SetVPOnRacePowerButton(int buttonIndex, int count)
    {
        if (buttonIndex >= racePowerButtons.Count)
        {
            Debug.LogError($"Index {buttonIndex} is out of range");
            return;
        }
        racePowerButtons[buttonIndex].UpdateVPCountLabel(count);
    }

    public void UpdatePlayerButton(int index)
    {
        if (index >= playerButtons.Count)
        {
            Debug.LogError($"Index {index} is out of range");
            return;
        }
        playerButtons[index].UpdateButtonUI();
    }

    public void AddRacePowerButtonListener(Action<SMRacePower> listener)
    {
        OnRacePowerClicked += listener;
    }

    public void RemoveRacePowerButtonListener(Action<SMRacePower> listener)
    {
        OnRacePowerClicked -= listener;
    }

    public void AddPlayerButtonListener(Action<SMPlayer> listener)
    {
        OnPlayerClicked += listener;
    }

    public void RemovePlayerButtonListener(Action<SMPlayer> listener)
    {
        OnPlayerClicked -= listener;
    }

    private void InitUIHooks()
    {
        var hooks = game.Hooks;
        var endTurnBtn = _doc.rootVisualElement.Q<Button>("EndTurnButton");
        var enterDeclineBtn = _doc.rootVisualElement.Q<Button>("EnterDeclineButton");
        var gameStatusText = _doc.rootVisualElement.Q<Label>("GameStatus");
        var gameHintText = _doc.rootVisualElement.Q<Label>("GameHint");

        hooks.Subscribe<BeforeRacePowerSelectionHook>(async (evt) =>
        {
            SetGameStatusText("RacePower selection phase");
        });

        hooks.Subscribe<BeforeConquerPhaseHook>(async (evt) =>
        {
            SetGameStatusText("Conquer phase");

            endTurnBtn.SetEnabled(true);
            enterDeclineBtn.SetEnabled(true);
        });

        hooks.Subscribe<AfterConquerRegionHook>(async (evt) =>
        {
            enterDeclineBtn.SetEnabled(evt.RacePower.CanEnterDecline());
        });

        hooks.Subscribe<AfterConquerPhaseHook>(async (evt) =>
        {
            endTurnBtn.SetEnabled(false);
            enterDeclineBtn.SetEnabled(false);
        });

        hooks.Subscribe<BeforeRedeployPhaseHook>(async (evt) =>
        {
            SetGameStatusText("Redeploy troops");
        });

        hooks.Subscribe<BeforeRedeployTroopsHook>(async (evt) =>
        {
            SetGameHintText($"You have {evt.RacePower.AvailableTokenCount} left to deploy");
        });

        hooks.Subscribe<AfterRedeployPhaseHook>(async (evt) =>
        {
            SetGameHintText($"{evt.Player.Name}'s turn is over");
            await Task.Delay(1500);
        });

        hooks.Subscribe<AfterScorePhaseHook>(async (evt) =>
        {
            SetGameStatusText("Score tallied");
            SetGameHintText($"{evt.Player.Name} scored {evt.VPScored} VP");

            await Task.Delay(1500);
        });
    }

    private void InitGameFlowButtons()
    {
        var startBtn = _doc.rootVisualElement.Q<Button>("StartGameButton");
        startBtn.clicked += OnStartGameClicked;
        startBtn.style.display = DisplayStyle.Flex;

        var endTurnBtn = _doc.rootVisualElement.Q<Button>("EndTurnButton");
        endTurnBtn.clicked += OnEndTurnClicked;
        endTurnBtn.style.display = DisplayStyle.None;

        var enterDeclineBtn = _doc.rootVisualElement.Q<Button>("EnterDeclineButton");
        enterDeclineBtn.clicked += OnEnterDeclineClicked;
        enterDeclineBtn.style.display = DisplayStyle.None;
    }

    private void OnStartGameClicked()
    {
        _doc.rootVisualElement.Q<Button>("StartGameButton").style.display = DisplayStyle.None;

        var endTurnBtn = _doc.rootVisualElement.Q<Button>("EndTurnButton");
        var enterDeclineBtn = _doc.rootVisualElement.Q<Button>("EnterDeclineButton");

        endTurnBtn.style.display = DisplayStyle.Flex;
        enterDeclineBtn.style.display = DisplayStyle.Flex;


        game.StartGame();
    }

    private void OnEndTurnClicked()
    {
        game.EndTurnClicked();
    }

    private void OnEnterDeclineClicked()
    {
        game.EnterDeclineClicked();
    }


    public void SetStartGameButtonEnabled(bool enabled)
    {
        var startBtn = _doc.rootVisualElement.Q<Button>("StartGameButton");
        startBtn.SetEnabled(enabled);
    }

    public void SetGameStatusText(string text)
    {
        var label = _doc.rootVisualElement.Q<Label>("GameStatus");
        label.text = text;
    }

    public void SetGameHintText(string text)
    {
        var label = _doc.rootVisualElement.Q<Label>("GameHint");
        label.text = text;
    }

}
