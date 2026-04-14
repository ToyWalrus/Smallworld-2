using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using SMGame = Smallworld.Models.Game;

namespace UnityModels
{
    public class Game : MonoBehaviour, IUnityModel<SMGame>
    {
        private SMGame model;

        public List<Player> Players;
        public List<Region> Regions;
        public List<RacePower> AvailableRacePowers;
        public int NumRounds;

        private readonly HashSet<Type> usedPowers = new();
        private readonly HashSet<Type> usedRaces = new();
        private int currentRound = 0;
        private int activePlayerIndex = 0;

        public Player ActivePlayer => Players[activePlayerIndex];

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public async Task RunGame()
        {
            while (currentRound < NumRounds)
            {
                while (activePlayerIndex < Players.Count)
                {

                    await SelectRacePowerComboPhase();
                    await ConquerPhase();

                    activePlayerIndex++;
                }

                currentRound++;
                activePlayerIndex = 0;
            }

            DetermineVictor();
        }

        private async Task SelectRacePowerComboPhase()
        {
            int availableVP = ActivePlayer.Score;
            var (rp, passedCount, existingVP) = await SelectRacePower(availableVP);

            var player = ActivePlayer.GetModel();
            player.AddScore(existingVP - passedCount);
            player.AddRacePower(rp.GetModel());

            RefreshRPList();
        }

        private async Task ConquerPhase(RacePower rp)
        {
            bool haveConquered = false;
            bool doneConquering = false;

            while (!doneConquering)
            {
                Region region = await SelectRegion();

                var regionModel = region.GetModel();
                int baseCost = regionModel.GetBaseConquerCost();
                regionModel.Conquer(rp.GetModel(), baseCost);

                haveConquered = true;
            }
        }

        public SMGame GetModel() => model;
    }
}
