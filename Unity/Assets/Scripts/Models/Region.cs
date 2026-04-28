using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Smallworld.Hooks;
using Smallworld.Models;
using Smallworld.Utils;
using UnityEngine;
using SMRegion = Smallworld.Models.Region;

namespace UnityModels
{
    public class Region : UnityModel<SMRegion>
    {
        public Rigidbody tilePrefab;

        [SerializeField] private RegionScriptableObject region;
        override public SMRegion GetModel() => region.GetModel();

        private Dictionary<Token, List<Rigidbody>> tiles = new();

        void Awake()
        {
            HooksService.Instance.Subscribe<RegionTokensAddedHook>(AddTokens);
            HooksService.Instance.Subscribe<RegionTokensRemovedHook>(RemoveTokens);
        }

        void OnDestroy()
        {
            HooksService.Instance.Unsubscribe<RegionTokensAddedHook>(AddTokens);
            HooksService.Instance.Unsubscribe<RegionTokensRemovedHook>(RemoveTokens);
        }

        async Task AddTokens(RegionTokensAddedHook evt)
        {
            if (evt.Region != GetModel()) return;

            for (int i = 0; i < evt.AddedCount; ++i)
            {
                var newTile = Instantiate(tilePrefab, transform);
                newTile.position = transform.position + .1f * (i + 2) * Vector3.up + .015f * i * Vector3.right;
                newTile.name = $"{evt.Token} {i + 1}";

                if (!tiles.ContainsKey(evt.Token))
                {
                    tiles.Add(evt.Token, new());
                }

                tiles[evt.Token].Add(newTile);
            }
        }

        async Task RemoveTokens(RegionTokensRemovedHook evt)
        {
            if (evt.Region != GetModel()) return;

            if (!tiles.ContainsKey(evt.Token))
            {
                Debug.LogError($"Tried removing ${evt.Token} from region, but no tokens of that type are registered there!");
                return;
            }

            foreach (var tile in tiles[evt.Token].Take(evt.RemovedCount))
            {
                Destroy(tile);
            }

            tiles[evt.Token].RemoveRange(0, evt.RemovedCount);
        }
    }
}