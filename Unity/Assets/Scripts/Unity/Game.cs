using System;
using System.Collections.Generic;
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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RunGame()
        {

        }

        public SMGame GetModel()
        {
            return model;
        }

    }

}
