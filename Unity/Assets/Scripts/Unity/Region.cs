using System.Collections.Generic;
using System.Linq;
using Smallworld.Models;
using UnityEngine;

using SMRegion = Smallworld.Models.Region;

namespace UnityModels
{
    public class Region : MonoBehaviour, IUnityModel<SMRegion>
    {
        private SMRegion model = new(RegionType.Hill, RegionAttribute.None, false, RegionAttribute.None);

        public RegionType Type
        {
            get => model.Type;
            set
            {
                model = new(value, Attribute, IsBorder, SecondAttribute);
            }
        }
        public RegionAttribute Attribute
        {
            get => model.Attribute;
            set
            {
                model = new(Type, value, IsBorder, SecondAttribute);
            }
        }
        public RegionAttribute SecondAttribute
        {
            get => model.SecondAttribute;
            set
            {
                model = new(Type, Attribute, IsBorder, value);
            }
        }
        public bool IsBorder
        {
            get => model.IsBorder;
            set
            {
                model.IsBorder = value;
            }
        }

        public bool IsOccupied => model.OccupiedBy != null || model.HasToken(Token.LostTribe);
        public int NumRaceTokens => model.NumRaceTokens;

        private List<Region> _adjacentTo;
        public List<Region> AdjacentTo
        {
            get => _adjacentTo;
            set
            {
                _adjacentTo = value;
                model.SetAdjacentRegions(value.Select((r) => r.GetModel()).ToList());
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() { }

        // Update is called once per frame
        void Update()
        {

        }

        public SMRegion GetModel()
        {
            return model;
        }
    }
}