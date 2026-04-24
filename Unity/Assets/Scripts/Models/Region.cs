using UnityEngine;
using SMRegion = Smallworld.Models.Region;

namespace UnityModels
{
    public class Region : UnityModel<SMRegion>
    {
        [SerializeField] private RegionScriptableObject region;
        override public SMRegion GetModel() => region.GetModel();
    }
}