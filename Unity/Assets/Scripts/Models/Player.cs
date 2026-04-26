using UnityEngine;

using SMPlayer = Smallworld.Models.Player;


namespace UnityModels
{
    public class Player : UnityModel<SMPlayer>
    {
        [SerializeField] private string PlayerName;

        private SMPlayer model = new("Player");

        void OnValidate()
        {
            model = new(PlayerName ?? name ?? "Player");
        }

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