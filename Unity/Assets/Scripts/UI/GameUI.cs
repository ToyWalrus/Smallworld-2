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


    public void SetRacePowerButtons(List<RacePower> racePowers)
    {
        var root = _doc.rootVisualElement;
        var container = root.Q<VisualElement>("RPListContainer");

        for (int i = 0; i < racePowers.Count; i++)
        {
            var rp = racePowers[i];

            // Get the instance (RP1, RP2, ...)
            var item = container.Q<VisualElement>($"RP{i + 1}");

            if (item == null)
                continue;

            item.SetEnabled(true);
            // Query inside the template instance
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
            item.SetEnabled(false);
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

    private void _OnRacePowerClicked(RacePower rp)
    {
        Debug.Log($"Clicked {rp.Name}");
        OnRacePowerClicked?.Invoke(rp);
    }
}
