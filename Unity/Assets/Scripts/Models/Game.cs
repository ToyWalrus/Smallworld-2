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
        [SerializeField] private List<RacePower> AvailableRacePowers;
        [SerializeField] private int NumRounds = 10;

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
            Debug.Log("Initializing hooks");
            HooksService.Initialize(new Hooks());
        }

        void Start()
        {
            var provider = ConfigureServiceProvider();
            game = new(provider, NumRounds);
            SetGameValues();

            playerSelector.gameUI.SetPlayerButtons(PlayerModels);

            Hooks.Subscribe<TurnStartHook>(async (evt) =>
            {
                Debug.Log($"Turn start for {evt.Player.Name}");
                playerSelector.gameUI.SetActivePlayerLabel(gameFlow.ActivePlayerIndex);
            });

            gameFlow = new(provider, game);

            // List<SMRacePower> rps = new();
            // for (int i = 0; i < 6; ++i)
            // {
            //     rps.Add(game.GenerateNewRacePower());
            // }

            // provider.GetRequiredService<ISelection<SMRacePower>>().SelectAsync(rps);

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
            game.SetAvailableRacePowers(AvailableRacePowers.Select(rp => rp.GetModel()).ToList());
        }
    }
}
