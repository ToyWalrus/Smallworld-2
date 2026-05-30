using System;
using System.Collections.Generic;
using System.Linq;
using Smallworld.Models.Races;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

using SMPlayer = Smallworld.Models.Player;
using SMRace = Smallworld.Models.Races.Race;


namespace UnityModels
{
    public class Player : UnityModel<SMPlayer>
    {
        [SerializeField, CreateProperty] private string PlayerName;

        // Unity attaches to this
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        private bool isCurrentPlayer = false;
        private Sprite inactiveBackgroundSprite;
        private Sprite activeBackgroundSprite;
        private VisualElement buttonRoot;

        [CreateProperty]
        private Sprite CurrentButtonBackground => isCurrentPlayer ? activeBackgroundSprite : inactiveBackgroundSprite;

        [CreateProperty]
        private string TokenCountString => model.ActiveRacePower == null ? "0" : model.ActiveRacePower.AvailableTokenCount.ToString();

        [CreateProperty]
        private string ActiveRacePowerName => model.ActiveRacePower == null ? "" : model.ActiveRacePower.Name;

        private SMPlayer model = new("Player");
        private List<Sprite> tokenSprites;

        void OnValidate()
        {
            model = new(PlayerName ?? name ?? "Player");
        }

        override public SMPlayer GetModel() => model;

        void Awake()
        {
            var uiSprites = Resources.LoadAll<Sprite>("UI-pack_Sprite_1");
            inactiveBackgroundSprite = uiSprites.FirstOrDefault(s => s.name == "InactivePlayerButtonBg");
            activeBackgroundSprite = uiSprites.FirstOrDefault(s => s.name == "ActivePlayerButtonBg");

            tokenSprites = Resources.LoadAll<Sprite>("racetokens").ToList();
        }

        public void SetIsCurrentPlayer(bool val)
        {
            isCurrentPlayer = val;
        }

        public void InitButton(VisualElement root)
        {
            buttonRoot = root;
            root.dataSource = this;

            var nameField = root.Q<Label>("PlayerName");
            nameField.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(PlayerName)),
                bindingMode = BindingMode.ToTarget
            });

            var rpLabel = root.Q<Label>("RacePowerLabel");
            rpLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(ActiveRacePowerName)),
                bindingMode = BindingMode.ToTarget
            });

            var tokenCountLabel = root.Q<Label>("TokenCount");
            tokenCountLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(TokenCountString)),
                bindingMode = BindingMode.ToTarget
            });

            UpdateTokenCountAreaVisibility();
            UpdateTokenImage();
            UpdateButtonBg();

            propertyChanged += RefreshNonTrivialProperties;
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
            var baseBtn = buttonRoot.Q<Button>("PlayerButton");
            baseBtn.style.backgroundImage = Background.FromSprite(CurrentButtonBackground);
        }

        private void UpdateTokenImage()
        {
            var img = buttonRoot.Q<Image>("RaceTokenImage");
            img.sprite = GetSpriteForRace(model.ActiveRacePower?.Race);
            img.style.visibility = img.sprite == null ? Visibility.Hidden : Visibility.Visible;
        }

        private void UpdateTokenCountAreaVisibility()
        {
            var tokenCountArea = buttonRoot.Q<VisualElement>("TokenCountArea");
            tokenCountArea.style.visibility = model.ActiveRacePower != null ? Visibility.Visible : Visibility.Hidden;
        }

        private void Notify(string propertyName)
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
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
}