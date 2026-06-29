using UnityEngine;
using SMPlayer = Smallworld.Models.Player;

namespace UnityModels
{
    public class Player : UnityModel<SMPlayer>
    {
        [SerializeField] private string PlayerName;

        private bool isCurrentPlayer = false;
        private SMPlayer model = new("Player");

        public string Name => PlayerName;
        public bool IsCurrentPlayer => isCurrentPlayer;

        void OnValidate()
        {
            model = new(PlayerName ?? name ?? "Player");
        }

        override public SMPlayer GetModel() => model;

        public void SetIsCurrentPlayer(bool val)
        {
            isCurrentPlayer = val;
        }
    }
}
