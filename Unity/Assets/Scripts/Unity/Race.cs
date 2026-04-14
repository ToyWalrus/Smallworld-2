using UnityEngine;

using SMRace = Smallworld.Models.Races.Race;

namespace UnityModels
{
    public class Race : MonoBehaviour, IUnityModel<SMRace>
    {
        private SMRace model;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public SMRace GetModel() => model;
    }
}