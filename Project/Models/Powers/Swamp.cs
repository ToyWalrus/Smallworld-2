using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Powers;

public class Swamp : Power
{
    public Swamp()
    {
        Name = "Swamp";
        StartingTokenCount = 4;
    }

    public override int TallyPowerBonusVP(List<Region> regions)
    {
        return IsInDecline ? 0 : regions.Count(region => region.Type == RegionType.Swamp);
    }
}
