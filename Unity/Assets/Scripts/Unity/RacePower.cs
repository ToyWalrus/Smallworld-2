using UnityEngine;

using SMRacePower = Smallworld.Models.RacePower;

namespace UnityModels
{
    public class RacePower : MonoBehaviour, IUnityModel<SMRacePower>
    {
        private SMRacePower model;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public SMRacePower GetModel()
        {
            return model;
        }
    }
}