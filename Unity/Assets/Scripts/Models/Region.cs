using System.Collections;
using System.Collections.Generic;
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
        public Material mountainTileMat;
        public RegionScriptableObject region;

        [SerializeField] private Vector3 HoverTransformOffset = Vector3.up;

        override public SMRegion GetModel() => region.GetModel();

        private Dictionary<Token, List<Rigidbody>> tiles = new();
        private bool isHovered = false;
        private Vector3 restingPosition;

        void Awake()
        {
            HooksService.Instance.Subscribe<RegionTokensAddedHook>(AddTokens);
            HooksService.Instance.Subscribe<RegionTokensRemovedHook>(RemoveTokens);
            restingPosition = transform.localPosition;

            if (GetModel().HasToken(Token.LostTribe))
            {
                InstantiateTokens(1, Token.LostTribe);
            }

            if (GetModel().HasToken(Token.Mountain))
            {
                InstantiateTokens(1, Token.Mountain);
                tiles[Token.Mountain][0].GetComponent<MeshRenderer>().material = mountainTileMat;
            }
        }

        void OnDestroy()
        {
            HooksService.Instance.Unsubscribe<RegionTokensAddedHook>(AddTokens);
            HooksService.Instance.Unsubscribe<RegionTokensRemovedHook>(RemoveTokens);
        }

        void Update()
        {
            var targetPosition = restingPosition + HoverTransformOffset;
            var threshold = 0.001f;
            if (isHovered && Vector3.Distance(transform.localPosition, targetPosition) > threshold)
            {
                transform.localPosition = targetPosition;
            }
            else if (!isHovered && Vector3.Distance(transform.localPosition, restingPosition) > threshold)
            {
                transform.localPosition = restingPosition;
            }
        }

        async Task AddTokens(RegionTokensAddedHook evt)
        {
            if (evt.Region != GetModel()) return;
            InstantiateTokens(evt.AddedCount, evt.Token);
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

        private void InstantiateTokens(int count, Token token)
        {
            var meshFilter = tilePrefab.GetComponentInChildren<MeshFilter>();
            var localSize = meshFilter != null ? meshFilter.sharedMesh.bounds.size : Vector3.one * 0.1f;
            var tileSize = Vector3.Scale(localSize, tilePrefab.transform.localScale);

            for (int i = 0; i < count; ++i)
            {
                var newTile = Instantiate(tilePrefab, transform);
                newTile.transform.localPosition = tileSize.y * (i + 2) * Vector3.up + tileSize.x * 0.15f * i * Vector3.right;
                newTile.name = $"{token} {i + 1}";

                if (!tiles.ContainsKey(token))
                {
                    tiles.Add(token, new());
                }

                tiles[token].Add(newTile);

                StartCoroutine(FreezeTileWhenSettled(newTile));
            }
        }

        public void SetIsHovered(bool hovered)
        {
            isHovered = hovered;
        }

        private IEnumerator FreezeTileWhenSettled(Rigidbody rb)
        {
            yield return new WaitUntil(() => rb.IsSleeping());
            rb.isKinematic = true;
        }
    }
}