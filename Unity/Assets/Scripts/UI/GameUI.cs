using System;
using System.Collections.Generic;
// using Smallworld.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityModels;
using SMPlayer = Smallworld.Models.Player;
using SMRacePower = Smallworld.Models.RacePower;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    public UnityEvent StartGameButtonPressed;
    public UnityEvent RollDieButtonPressed;

    private event Action<SMRacePower> OnRacePowerClicked;
    private event Action<SMPlayer> OnPlayerClicked;
    private UIDocument _doc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _doc = GetComponent<UIDocument>();

        var startBtn = _doc.rootVisualElement.Q<Button>("StartGameButton");
        startBtn.clicked += OnStartGameClicked;

        var rollBtn = _doc.rootVisualElement.Q<Button>("RollDieButton");
        rollBtn.clicked += OnRollDieButtonClicked;
    }

    public void SetRacePowerButtonsInteractable(bool interactable)
    {
        _doc.rootVisualElement.Q<VisualElement>("RPListContainer").SetEnabled(interactable);
    }

    public void SetActivePlayerLabel(int playerIndex)
    {
        var root = _doc.rootVisualElement;
        var container = root.Q<VisualElement>("PlayerContainer");

        for (int i = 0; i < container.childCount; ++i)
        {
            var item = container.Q<VisualElement>($"P{i + 1}");
            var activeLabel = item.Q<Label>("ActiveLabel");

            if (i == playerIndex)
            {
                activeLabel.style.visibility = Visibility.Visible;
            }
            else
            {
                activeLabel.style.visibility = Visibility.Hidden;
            }
        }
    }

    public void SetPlayerRacePower(int playerIndex, SMRacePower rp)
    {
        var root = _doc.rootVisualElement;
        var container = root.Q<VisualElement>("PlayerContainer");
        var item = container.Q<VisualElement>($"P{playerIndex + 1}");
        var rpLabel = item.Q<Label>("RacePowerLabel");

        rpLabel.text = rp.Name;
        UpdatePlayerTokenCount(playerIndex, rp.AvailableTokenCount);
    }

    public void SetRacePowerButtons(List<SMRacePower> racePowers)
    {
        var root = _doc.rootVisualElement;
        var container = root.Q<VisualElement>("RPListContainer");

        for (int i = 0; i < racePowers.Count; ++i)
        {
            UpdateRacePowerButton(i, racePowers[i]);
        }

        var extraCount = container.childCount - racePowers.Count;
        while (extraCount > 0)
        {
            var item = container.Q<VisualElement>($"RP{6 - extraCount + 1}");
            item.style.display = DisplayStyle.None;
            extraCount--;
        }
    }

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

    private void OnStartGameClicked()
    {
        StartGameButtonPressed.Invoke();
    }

    private void OnRollDieButtonClicked()
    {
        RollDieButtonPressed.Invoke();
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
