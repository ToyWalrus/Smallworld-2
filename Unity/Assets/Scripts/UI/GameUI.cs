using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Smallworld.Models;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    private event Action<RacePower> OnRacePowerClicked;
    private event Action<Player> OnPlayerClicked;
    private UIDocument _doc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _doc = GetComponent<UIDocument>();
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

    public void SetRacePowerButtons(List<RacePower> racePowers)
    {
        var root = _doc.rootVisualElement;
        var container = root.Q<VisualElement>("RPListContainer");

        for (int i = 0; i < racePowers.Count; ++i)
        {
            var rp = racePowers[i];
            var item = container.Q<VisualElement>($"RP{i + 1}");

            if (item == null)
                continue;

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

        var extraCount = container.childCount - racePowers.Count;
        while (extraCount > 0)
        {
            var item = container.Q<VisualElement>($"RP{6 - extraCount + 1}");
            item.style.display = DisplayStyle.None;
            extraCount--;
        }
    }

    public void SetPlayerButtons(List<Player> players)
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

            var button = item.Q<Button>("PlayerButton");
            var playerName = item.Q<Label>("PlayerName");
            var activePlayerLabel = item.Q<Label>("ActiveLabel");

            playerName.text = player.Name ?? $"Player {i + 1}";
            activePlayerLabel.style.visibility = Visibility.Hidden;

            // Clear previous handlers (important if reused)
            button.clicked -= () => _OnPlayerClicked(player);

            // Register click
            button.clicked += () => _OnPlayerClicked(player);
        }

        var extraCount = container.childCount - players.Count;
        while (extraCount > 0)
        {
            var item = container.Q<VisualElement>($"P{4 - extraCount + 1}");
            item.style.display = DisplayStyle.None;
            extraCount--;
        }
    }


    public void AddRacePowerButtonListener(Action<RacePower> listener)
    {
        OnRacePowerClicked += listener;
    }

    public void RemoveRacePowerButtonListener(Action<RacePower> listener)
    {
        OnRacePowerClicked -= listener;
    }

    public void AddPlayerButtonListener(Action<Player> listener)
    {
        OnPlayerClicked += listener;
    }

    public void RemovePlayerButtonListener(Action<Player> listener)
    {
        OnPlayerClicked -= listener;
    }

    private void _OnRacePowerClicked(RacePower rp)
    {
        Debug.Log($"Clicked {rp.Name}");
        OnRacePowerClicked?.Invoke(rp);
    }

    private void _OnPlayerClicked(Player player)
    {
        Debug.Log($"Clicked {player.Name}");
        OnPlayerClicked?.Invoke(player);
    }
}
