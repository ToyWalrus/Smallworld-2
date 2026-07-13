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
        // public Rigidbody tilePrefab;
        public RegionScriptableObject region;
        [SerializeField] private Sprite regionShape;
        [SerializeField] private float targetOverlayTransparency = .9f;
        [SerializeField] private float overlayFadeSpeed = 6f;
        [SerializeField] private Material whiteMaterial;


        override public SMRegion GetModel() => region.GetModel();


        private bool isHovered = false;
        private bool isHighlighted = false;
        private SpriteRenderer overlayRenderer;
        private Material ogMaterial;


        void Awake()
        {
            HooksService.Instance.Subscribe<RegionTokensAddedHook>(AddTokens);
            HooksService.Instance.Subscribe<RegionTokensRemovedHook>(RemoveTokens);

            if (GetModel().HasToken(Token.LostTribe))
            {
                InstantiateTokens(1, Token.LostTribe);
            }

            if (GetModel().HasToken(Token.Mountain))
            {
                InstantiateTokens(1, Token.Mountain);
            }

            if (regionShape != null && TryGetComponent<PolygonCollider2D>(out var collider))
            {
                var shapeChild = new GameObject($"{name}_Shape");

                overlayRenderer = shapeChild.AddComponent<SpriteRenderer>();
                overlayRenderer.sprite = regionShape;
                overlayRenderer.sortingLayerName = "RegionShapes";
                overlayRenderer.color = new Color(1f, 1f, 1f, 0f);

                ogMaterial = overlayRenderer.material;

                RegionOverlayUtil.SnapOverlayToCollider(collider, overlayRenderer);
            }
        }

        void OnDestroy()
        {
            HooksService.Instance.Unsubscribe<RegionTokensAddedHook>(AddTokens);
            HooksService.Instance.Unsubscribe<RegionTokensRemovedHook>(RemoveTokens);
        }

        void Update()
        {
            UpdateOverlayRenderer();
        }

        async Task AddTokens(RegionTokensAddedHook evt)
        {
            if (evt.Region != GetModel()) return;
            InstantiateTokens(evt.AddedCount, evt.Token);
        }

        async Task RemoveTokens(RegionTokensRemovedHook evt)
        {
            if (evt.Region != GetModel()) return;
            DestroyTokens(evt.RemovedCount, evt.Token);
        }

        private void InstantiateTokens(int count, Token token)
        {
            // var meshFilter = tilePrefab.GetComponentInChildren<MeshFilter>();
            // var localSize = meshFilter != null ? meshFilter.sharedMesh.bounds.size : Vector3.one * 0.1f;
            // var tileSize = Vector3.Scale(localSize, tilePrefab.transform.localScale);

            // for (int i = 0; i < count; ++i)
            // {
            //     var newTile = Instantiate(tilePrefab, transform);
            //     newTile.transform.localPosition = tileSize.y * (i + 2) * Vector3.up + tileSize.x * 0.15f * i * Vector3.right;
            //     newTile.name = $"{token} {i + 1}";

            //     if (!tiles.ContainsKey(token))
            //     {
            //         tiles.Add(token, new());
            //     }

            //     tiles[token].Add(newTile);

            //     StartCoroutine(FreezeTileWhenSettled(newTile));
            // }
        }

        private void DestroyTokens(int count, Token token)
        {
            // if (!tiles.ContainsKey(token))
            // {
            //     Debug.LogError($"Tried removing {token} from region, but no tokens of that type are registered there!");
            //     return;
            // }

            // foreach (var tile in tiles[token].Take(count))
            // {
            //     Destroy(tile);
            // }

            // tiles[token].RemoveRange(0, count);
        }

        public void SetIsHovered(bool hovered)
        {
            isHovered = hovered;
        }

        public void SetIsHighlighted(bool highlighted)
        {
            isHighlighted = highlighted;
        }

        private void UpdateOverlayRenderer()
        {
            if (overlayRenderer == null)
            {
                return;
            }

            var targetAlpha = isHighlighted || isHovered ? targetOverlayTransparency : 0f;
            var targetSpeed = isHovered ? 0 : overlayFadeSpeed;
            var newAlpha = Mathf.MoveTowards(overlayRenderer.color.a, targetAlpha, targetSpeed * Time.deltaTime);
            overlayRenderer.color = new Color(1f, 1f, 1f, newAlpha);

            if (isHovered && whiteMaterial != null)
            {
                overlayRenderer.material = whiteMaterial;
            }
            else if (!isHovered)
            {
                overlayRenderer.material = ogMaterial;
            }
        }
    }
}