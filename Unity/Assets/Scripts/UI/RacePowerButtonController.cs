using System;
using System.Collections.Generic;
using System.Linq;
using Smallworld.Models.Races;
using UnityEngine;
using UnityEngine.UIElements;
using SMRace = Smallworld.Models.Races.Race;
using SMRacePower = Smallworld.Models.RacePower;

public class RacePowerButtonController
{
    private SMRacePower model;
    public SMRacePower GetModel() => model;

    private List<Sprite> tokenSprites;

    public event Action<SMRacePower> OnClicked;
    private VisualElement root;

    public RacePowerButtonController(VisualElement root, List<Sprite> tokenSprites)
    {
        this.root = root;
        this.tokenSprites = tokenSprites;
    }

    public void InitButton(SMRacePower rp)
    {
        model = rp;

        root.Q<Label>("RaceLabel").text = rp.Race.Name;
        root.Q<Label>("PowerLabel").text = rp.Power.Name;
        root.Q<Image>("RaceTokenImage").sprite = GetSpriteForRace(rp.Race);
        UpdateVPCountLabel(0);

        var button = root.Q<Button>("RPButton");
        button.clicked += () => OnClicked?.Invoke(model);
    }

    public void RemoveSelf(VisualElement parent)
    {
        parent.Remove(root);
    }

    public void UpdateVPCountLabel(int count)
    {

        if (count <= 0)
        {
            root.Q<VisualElement>("VPBadge").style.display = DisplayStyle.None;
        }
        else
        {
            root.Q<VisualElement>("VPBadge").style.display = DisplayStyle.Flex;
            root.Q<Label>("VPAmount").text = count.ToString();
        }
    }

    public void SetEnabled(bool enabled)
    {
        root.Q<Button>("RPButton").SetEnabled(enabled);
    }

    // TODO: extract this somewhere, copied from Player
    private Sprite GetSpriteForRace(SMRace race)
    {
        if (tokenSprites == null || race == null)
        {
            return null;
        }

        var spriteName = "Token_";
        switch (race)
        {
            case Amazon:
                spriteName += "Amazon";
                break;
            case Dwarf:
                spriteName += "Dwarf";
                break;
            case Elf:
                spriteName += "Elf";
                break;
            case Ghoul:
                spriteName += "Ghoul";
                break;
            case Giant:
                spriteName += "Giant";
                break;
            case Halfling:
                spriteName += "Halfling";
                break;
            case Human:
                spriteName += "Human";
                break;
            case Orc:
                spriteName += "Orc";
                break;
            case Ratmen:
                spriteName += "Ratmen";
                break;
            case Skeleton:
                spriteName += "Skeleton";
                break;
            case Sorcerer:
                spriteName += "Sorcerer";
                break;
            case Triton:
                spriteName += "Triton";
                break;
            case Troll:
                spriteName += "Troll";
                break;
            case Wizard:
                spriteName += "Wizard";
                break;
            default:
                return null;
        }


        return tokenSprites.FirstOrDefault(s => s.name == spriteName);
    }


}