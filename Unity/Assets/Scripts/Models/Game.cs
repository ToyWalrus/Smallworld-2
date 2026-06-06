using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.Hooks;
using Smallworld.IO;
using Smallworld.Logic;
using Smallworld.Models;
using Smallworld.Utils;
using Unity.VisualScripting;
using UnityEngine;

using SMGame = Smallworld.Models.Game;
using SMPlayer = Smallworld.Models.Player;
using SMRacePower = Smallworld.Models.RacePower;
using SMRegion = Smallworld.Models.Region;

namespace UnityModels
{
    public class Game : MonoBehaviour
    {
        private SMGame game;

        [SerializeField] private List<Player> Players;
        [SerializeField] private List<Region> Regions;
        [SerializeField] private int NumRounds = 10;
        [SerializeField] private GameUI GameUI;

        public DiceRoller diceRoller;
        public ConfirmationPopup confirmationPopup;
        public RegionSelector regionSelector;
        public RacePowerSelector racePowerSelector;
        public PlayerSelector playerSelector;

        public IHooks Hooks => HooksService.Instance;

        private List<SMPlayer> PlayerModels => Players.Select(p => p.GetModel()).ToList();
        private GameFlow gameFlow;


        void OnValidate()
        {
            SetGameValues();
        }

        void Awake()
        {
            // Game.cs is set to execute before any other asset script
            Smallworld.Utils.Logger.SetType<UnityLogger>();
            HooksService.Initialize(new Hooks());
        }

        void Start()
        {
            var provider = ConfigureServiceProvider();
            game = new(provider, NumRounds);
            SetGameValues();

            Hooks.Subscribe<TurnStartHook>(async (evt) =>
            {
                Debug.Log($"Turn start for {evt.Player.Name}");
                var activePlayer = Players[gameFlow.ActivePlayerIndex];
                activePlayer.SetIsCurrentPlayer(true);
                activePlayer.UpdateButtonUI();

                if (gameFlow.ActivePlayer.ActiveRacePower != null)
                {
                    // GameUI.UpdatePlayerTokenCount(gameFlow.ActivePlayerIndex, gameFlow.ActivePlayer.ActiveRacePower.AvailableTokenCount);
                }
            });

            Hooks.Subscribe<TurnEndHook>(async (evt) =>
            {
                var activePlayer = Players[gameFlow.ActivePlayerIndex];
                activePlayer.SetIsCurrentPlayer(false);
                activePlayer.UpdateButtonUI();
            });

            Hooks.Subscribe<AfterRacePowerSelectionHook>(async (evt) =>
            {
                Players[gameFlow.ActivePlayerIndex].UpdateButtonUI();

                // Replace the selected race power button with a new one
                GameUI.ReplaceRacePowerButton(evt.SelectedIndex, game.AvailableRacePowers[evt.SelectedIndex]);

                // TODO: This is bad, get actual count
                for (int i = 0; i < 6; ++i)
                {
                    GameUI.SetVPOnRacePowerButton(i, gameFlow.GetVPOnRacePowerIndex(i));
                }
            });

            Hooks.Subscribe<AfterConquerRegionHook>(async (evt) =>
            {
                foreach (var p in Players)
                {
                    p.UpdateButtonUI();
                }
                // GameUI.UpdatePlayerTokenCount(gameFlow.ActivePlayerIndex, evt.RacePower.AvailableTokenCount);
            });

            // This probably belongs in the game logic
            List<SMRacePower> rps = new();
            for (int i = 0; i < 6; ++i)
            {
                rps.Add(game.GenerateNewRacePower());
            }
            game.SetAvailableRacePowers(rps);

            GameUI.InitRacePowerButtons(rps);
            GameUI.InitPlayerButtons(Players);

            gameFlow = new(provider, game);
        }

        public void StartGame()
        {
            gameFlow.RunGame();
        }

        private ServiceProvider ConfigureServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IGame, SMGame>((_) => game);
            services.AddTransient<IRollDice>((_) => diceRoller);
            services.AddTransient<IConfirmation>((_) => confirmationPopup);
            services.AddTransient<ISelection<SMRegion>>((_) => regionSelector);
            services.AddTransient<ISelection<SMRacePower>>((_) => racePowerSelector);
            services.AddTransient<ISelection<SMPlayer>>((_) => playerSelector);

            return services.BuildServiceProvider();
        }

        private void SetGameValues()
        {
            if (game == null) return;

            game.NumRounds = NumRounds;
            game.SetPlayers(PlayerModels);
            game.SetRegions(Regions.Select(r => r.GetModel()).ToList());
        }

        public void EndTurnClicked()
        {
            gameFlow.DoneConqueringButtonPressed();
        }

        public void EnterDeclineClicked()
        {
            gameFlow.EnterDeclineButtonPressed();
        }
    }
}
