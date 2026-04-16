using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Smallworld.Logic;
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
        [SerializeField] private int NumRounds;

        private readonly HashSet<Type> usedPowers = new();
        private readonly HashSet<Type> usedRaces = new();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartGame()
        {
            var flow = new GameFlow();
            flow.RunGame(model);
        }


        public SMGame GetModel() => model;
    }
}
