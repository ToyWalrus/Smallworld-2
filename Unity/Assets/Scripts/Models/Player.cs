using System;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using SMPlayer = Smallworld.Models.Player;


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

        void OnValidate()
        {
            model = new(PlayerName ?? name ?? "Player");
        }

        override public SMPlayer GetModel() => model;

        void Awake()
        {
            var allSprites = Resources.LoadAll<Sprite>("UI-pack_Sprite_1");
            Debug.Log(allSprites.Count());
            inactiveBackgroundSprite = allSprites.FirstOrDefault(s => s.name == "InactivePlayerButtonBg");
            activeBackgroundSprite = allSprites.FirstOrDefault(s => s.name == "ActivePlayerButtonBg");
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
            }
        }

        private void UpdateButtonBg()
        {
            var baseBtn = buttonRoot.Q<Button>("PlayerButton");
            baseBtn.style.backgroundImage = Background.FromSprite(CurrentButtonBackground);
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
    }
}