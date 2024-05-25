using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class Underworld : Power
{
    public Underworld()
    {
        Name = "Underworld";
        StartingTokenCount = 5;
    }

    public override Task<int> GetRegionConquerCostReduction(Region region)
    {
        return Task.FromResult(region.HasAttribute(RegionAttribute.Underworld) ? 1 : 0);
    }

    public override List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region)
    {
        var reasons = region.GetInvalidConquerReasons(ownedRegions);
        if (
            reasons.Contains(InvalidConquerReason.NotAdjacent) &&
            ownedRegions.Any(r => r.HasAttribute(RegionAttribute.Underworld)) &&
            region.HasAttribute(RegionAttribute.Underworld)
        )
        {
            reasons.Remove(InvalidConquerReason.NotAdjacent);
        }
        return reasons;
    }
}
