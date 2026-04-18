using UnityEngine;

using SMPlayer = Smallworld.Models.Player;


namespace UnityModels
{
    public class Player : UnityModel<SMPlayer>
    {
        private string Name
        {
            get => model.Name;
            set { model.Name = value; }
        }
        public int Score => model.Score;

        private SMPlayer model = new("Player");

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        override public SMPlayer GetModel() => model;
    }
}