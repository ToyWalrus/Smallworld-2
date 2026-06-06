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

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        InitGameFlowButtons();
        InitUIHooks();
    }

    public void SetRacePowerButtonsInteractable(bool interactable)
    {
        _doc.rootVisualElement.Q<VisualElement>("RPContainer").SetEnabled(interactable);
    }

    public void InitRacePowerButtons(List<SMRacePower> racePowers)
    {
        var area = _doc.rootVisualElement.Q<VisualElement>("RPContainer");
        var tokenSprites = Resources.LoadAll<Sprite>("racetokens").ToList();
        foreach (var rp in racePowers)
        {
            var el = racePowerButton.Instantiate();
            var controller = new RacePowerButtonController(el, tokenSprites);
            controller.InitButton(rp);

            area.Add(el);
            racePowerButtons.Add(controller);
            controller.OnClicked += (rp) => OnRacePowerClicked?.Invoke(rp);
        }
    }

    // TODO: This architecture sucks.
    public void SetRacePowerButtons(List<SMRacePower> racePowers)
    {
        var area = _doc.rootVisualElement.Q<VisualElement>("RPContainer");
        foreach (var btn in racePowerButtons)
        {
            btn.RemoveSelf(area);
        }

        racePowerButtons.Clear();
        InitRacePowerButtons(racePowers);
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

    // TODO: move to Player script
    public void InitPlayerButtons(List<Player> players)
    {
        var root = _doc.rootVisualElement;
        var container = root.Q<VisualElement>("PlayerContainer");


        for (int i = 0; i < players.Count; ++i)
        {
            var player = players[i];
            var item = container.Q<VisualElement>($"P{i + 1}");


            if (item == null)
                continue;

            item.style.display = DisplayStyle.Flex;

            player.InitButton(item);
            var button = item.Q<Button>("PlayerButton");

            // var playerName = item.Q<Label>("PlayerName");
            // var activePlayerLabel = item.Q<Label>("ActiveLabel");
            // var rpLabel = item.Q<Label>("RacePowerLabel");
            // var tokenCountArea = item.Q<VisualElement>("TokenCountArea");

            // playerName.text = player.Name ?? $"Player {i + 1}";
            // activePlayerLabel.style.visibility = Visibility.Hidden;
            // rpLabel.text = "";
            // tokenCountArea.style.display = DisplayStyle.None;

            // Clear previous handlers (important if reused)
            button.clicked -= () => _OnPlayerClicked(player.GetModel());

            // Register click
            button.clicked += () => _OnPlayerClicked(player.GetModel());
        }

        var extraCount = container.childCount - players.Count;
        while (extraCount > 0)
        {
            var item = container.Q<VisualElement>($"P{4 - extraCount + 1}");
            item.style.display = DisplayStyle.None;
            extraCount--;
        }
    }

    public void UpdateRacePowerButton(int index, SMRacePower rp)
    {
        var container = _doc.rootVisualElement.Q<VisualElement>("RPListContainer");
        var item = container.Q<VisualElement>($"RP{index + 1}");

        if (item == null)
        {
            return;
        }

        item.style.display = DisplayStyle.Flex;

        var button = item.Q<Button>("RPButton");
        var raceLabel = item.Q<Label>("RaceLabel");
        var powerLabel = item.Q<Label>("PowerLabel");

        // Set text
        raceLabel.text = rp.Race.Name;
        powerLabel.text = rp.Power.Name;

        // Clear previous handlers (important if reused)
        button.clicked -= () => _OnRacePowerClicked(rp);

        // Register click
        button.clicked += () => _OnRacePowerClicked(rp);
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

    private void _OnRacePowerClicked(SMRacePower rp)
    {
        Debug.Log($"Clicked {rp.Name}");
        OnRacePowerClicked?.Invoke(rp);
    }

    private void _OnPlayerClicked(SMPlayer player)
    {
        Debug.Log($"Clicked {player.Name}");
        OnPlayerClicked?.Invoke(player);
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

        hooks.Subscribe<AfterScorePhaseHook>(async (evt) =>
        {
            SetGameStatusText("Score tallied");
            SetGameHintText($"{evt.Player.Name} scored ${evt.VPScored} VP");

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

    public void UpdatePlayerTokenCount(int playerIndex, int count)
    {
        var playerElement = _doc.rootVisualElement.Q("PlayerContainer").Q($"P{playerIndex + 1}");
        var tokenCount = playerElement.Q<Label>("TokenCount");

        playerElement.Q("TokenCountArea").style.display = DisplayStyle.Flex;
        tokenCount.text = count.ToString();
    }
}
