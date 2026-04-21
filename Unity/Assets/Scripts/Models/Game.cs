using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.Hooks;
using Smallworld.IO;
using Smallworld.Models;
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

        public IHooks Hooks => game.Hooks;

        void OnValidate()
        {
            if (game == null) return;

            game.NumRounds = NumRounds;
            game.SetPlayers(Players.Select(p => p.GetModel()).ToList());
            game.SetRegions(Regions.Select(r => r.GetModel()).ToList());
            game.SetAvailableRacePowers(AvailableRacePowers.Select(rp => rp.GetModel()).ToList());
        }


        void Start()
        {
            game = new(ConfigureServiceProvider(), NumRounds);
        }

        void Update()
        {

        }

        public void StartGame()
        {

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
    }
}
