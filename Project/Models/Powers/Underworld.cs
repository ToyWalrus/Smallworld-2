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

    public override int GetEstimatedConquerCostReduction(Region region)
    {
        return region.HasAttribute(RegionAttribute.Underworld) ? 1 : 0;
    }

    public override void ModifyConquerRestrictions(List<InvalidConquerReason> reasons, List<Region> ownedRegions, Region region)
    {
        if (
            reasons.Contains(InvalidConquerReason.NotAdjacent) &&
            ownedRegions.Any(r => r.HasAttribute(RegionAttribute.Underworld)) &&
            region.HasAttribute(RegionAttribute.Underworld)
        )
        {
            reasons.Remove(InvalidConquerReason.NotAdjacent);
        }
    }
}
