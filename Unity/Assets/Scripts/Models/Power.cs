using UnityEngine;

using SMPower = Smallworld.Models.Powers.Power;


namespace UnityModels
{
    public class Power : IUnityModel<SMPower>
    {
        private SMPower model;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        override public SMPower GetModel() => model;
    }
}