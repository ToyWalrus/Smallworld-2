using UnityEngine;

using SMPlayer = Smallworld.Models.Player;


namespace UnityModels
{
    public class Player : IUnityModel<SMPlayer>
    {
        public int Score => model.Score;

        private SMPlayer model;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            model = new(this.name);
        }

        // Update is called once per frame
        void Update()
        {

        }

        override public SMPlayer GetModel() => model;
    }
}