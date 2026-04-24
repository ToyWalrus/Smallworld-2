using System.Collections.Generic;
using System.Linq;
using Smallworld.Models;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using SMRegion = Smallworld.Models.Region;

[CreateAssetMenu(fileName = "Region", menuName = "Region")]
public class RegionScriptableObject : ScriptableObject
{
    private static bool isSyncing = false;

    private SMRegion model = null;

    [SerializeField] private RegionType regionType = RegionType.Hill;
    [SerializeField] private RegionAttribute attribute = RegionAttribute.None;
    [SerializeField] private RegionAttribute secondaryAttribute = RegionAttribute.None;
    [SerializeField] private bool isBorder = false;
    [SerializeField] private bool lostTribeRegion = false;
    [SerializeField] private List<RegionScriptableObject> adjacentRegions = new();

    [SerializeField, HideInInspector] private List<RegionScriptableObject> oldAdjacentRegions = new();

    void OnValidate()
    {
        SyncAdjacency();
        RebuildModel();
    }

    private void RebuildModel()
    {
        model = new(regionType, attribute, isBorder, secondaryAttribute);

        if (adjacentRegions != null)
        {
            model.SetAdjacentRegions(
                adjacentRegions
                    .Where(r => r != null)
                    .Select(r => r.GetModel())
                    .ToList()
            );
        }

        if (lostTribeRegion)
        {
            model.AddToken(Token.LostTribe);
        }
        else
        {
            model.Abandon();
        }
    }

    private void SyncAdjacency()
    {
        if (isSyncing) return;

        try
        {
            isSyncing = true;

            adjacentRegions ??= new();
            oldAdjacentRegions ??= new();

            var current = new HashSet<RegionScriptableObject>(adjacentRegions.Where(r => r != null && r != this));
            var previous = new HashSet<RegionScriptableObject>(oldAdjacentRegions.Where(r => r != null && r != this));

            // Update neighbors
            foreach (var added in current.Except(previous))
            {
                added.adjacentRegions ??= new();

                if (!added.adjacentRegions.Contains(this))
                {
                    added.adjacentRegions.Add(this);

#if UNITY_EDITOR
                    EditorUtility.SetDirty(added);
#endif
                }
            }

            // Remove neighbors
            foreach (var removed in previous.Except(current))
            {
                if (removed.adjacentRegions != null && removed.adjacentRegions.Contains(this))
                {
                    removed.adjacentRegions.Remove(this);

#if UNITY_EDITOR
                    EditorUtility.SetDirty(removed);
#endif
                }
            }

            // Update snapshot
            oldAdjacentRegions = new List<RegionScriptableObject>(adjacentRegions);
        }
        finally
        {
            isSyncing = false;
        }
    }

    public SMRegion GetModel()
    {
        if (model == null)
        {
            RebuildModel();
        }
        return model;
    }
}
