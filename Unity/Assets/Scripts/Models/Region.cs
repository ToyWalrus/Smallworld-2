using System.Collections.Generic;
using System.Linq;
using Smallworld.Models;
using UnityEngine;

using SMRegion = Smallworld.Models.Region;

namespace UnityModels
{
    public class Region : UnityModel<SMRegion>
    {
        [SerializeField] private RegionType type = RegionType.Hill;
        [SerializeField] private RegionAttribute attribute = RegionAttribute.None;
        [SerializeField] private RegionAttribute secondAttribute = RegionAttribute.None;
        [SerializeField] private bool isBorder;

        private SMRegion model;

        void Awake() => RebuildModel();
        void OnValidate() => RebuildModel();

        public bool IsOccupied => model.IsOccupied;
        public int NumRaceTokens => model.NumRaceTokens;

        private List<Region> _adjacentTo;
        public List<Region> AdjacentTo
        {
            get => _adjacentTo;
            set
            {
                _adjacentTo = value;
                model.SetAdjacentRegions(value.Select(r => r.GetModel()).ToList());
            }
        }

        override public SMRegion GetModel() => model;

        private void RebuildModel()
        {
            model = new SMRegion(type, attribute, isBorder, secondAttribute);
        }

    }
}