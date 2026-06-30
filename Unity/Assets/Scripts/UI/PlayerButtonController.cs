using System;
using System.Collections.Generic;
using System.Linq;
using Smallworld.Models.Races;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using UnityModels;
using SMPlayer = Smallworld.Models.Player;
using SMRace = Smallworld.Models.Races.Race;

public class PlayerButtonController
{
    private Player player;
    private VisualElement root;
    private Sprite inactiveBackgroundSprite;
    private Sprite activeBackgroundSprite;
    private List<Sprite> tokenSprites;

    public event Action<SMPlayer> OnClicked;

    // Unity attaches to this
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    [CreateProperty]
    private string PlayerName => player.Name;

    [CreateProperty]
    private Sprite CurrentButtonBackground => player.IsCurrentPlayer ? activeBackgroundSprite : inactiveBackgroundSprite;

    [CreateProperty]
    private string TokenCountString => player.GetModel().ActiveRacePower == null ? "0" : player.GetModel().ActiveRacePower.AvailableTokenCount.ToString();

    [CreateProperty]
    private string ActiveRacePowerName => player.GetModel().ActiveRacePower == null ? "" : player.GetModel().ActiveRacePower.Name;

    public PlayerButtonController(VisualElement root, Player player)
    {
        this.root = root;
        this.player = player;

        var uiSprites = Resources.LoadAll<Sprite>("UI-pack_Sprite_1");
        inactiveBackgroundSprite = uiSprites.FirstOrDefault(s => s.name == "InactivePlayerButtonBg");
        activeBackgroundSprite = uiSprites.FirstOrDefault(s => s.name == "ActivePlayerButtonBg");
        tokenSprites = Resources.LoadAll<Sprite>("racetokens").ToList();

        root.dataSource = this;

        root.Q<Label>("PlayerName").SetBinding("text", new DataBinding()
        {
            dataSourcePath = new PropertyPath(nameof(PlayerName)),
            bindingMode = BindingMode.ToTarget
        });

        root.Q<Label>("RacePowerLabel").SetBinding("text", new DataBinding()
        {
            dataSourcePath = new PropertyPath(nameof(ActiveRacePowerName)),
            bindingMode = BindingMode.ToTarget
        });

        root.Q<Label>("TokenCount").SetBinding("text", new DataBinding()
        {
            dataSourcePath = new PropertyPath(nameof(TokenCountString)),
            bindingMode = BindingMode.ToTarget
        });

        UpdateTokenCountAreaVisibility();
        UpdateTokenImage();
        UpdateButtonBg();

        propertyChanged += RefreshNonTrivialProperties;

        root.style.display = DisplayStyle.Flex;
        root.Q<Button>("PlayerButton").clicked += _onButtonClicked;
    }

    public void UpdateButtonUI()
    {
        Notify(nameof(PlayerName));
        Notify(nameof(CurrentButtonBackground));
        Notify(nameof(ActiveRacePowerName));
        Notify(nameof(TokenCountString));
    }

    private void RefreshNonTrivialProperties(object obj, BindablePropertyChangedEventArgs args)
    {
        switch (args.propertyName)
        {
            case nameof(CurrentButtonBackground):
                UpdateButtonBg();
                break;
            case nameof(TokenCountString):
                UpdateTokenCountAreaVisibility();
                break;
            case nameof(ActiveRacePowerName):
                UpdateTokenImage();
                break;
        }
    }

    private void UpdateButtonBg()
    {
        root.Q<Button>("PlayerButton").style.backgroundImage = Background.FromSprite(CurrentButtonBackground);
    }

    private void UpdateTokenImage()
    {
        var img = root.Q<Image>("RaceTokenImage");
        img.sprite = GetSpriteForRace(player.GetModel().ActiveRacePower?.Race);
        img.style.visibility = img.sprite == null ? Visibility.Hidden : Visibility.Visible;
    }

    private void UpdateTokenCountAreaVisibility()
    {
        root.Q<VisualElement>("TokenCountArea").style.visibility = player.GetModel().ActiveRacePower != null ? Visibility.Visible : Visibility.Hidden;
    }

    private void Notify(string propertyName)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
    }

    private void _onButtonClicked()
    {
        OnClicked?.Invoke(player.GetModel());
    }

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
