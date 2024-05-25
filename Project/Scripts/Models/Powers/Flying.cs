using System.Collections.Generic;

namespace Smallworld.Models.Powers;

public class Flying : Power
{
    public Flying()
    {
        Name = "Flying";
        StartingTokenCount = 5;
    }

    public override List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region)
    {
        var reasons = region.GetInvalidConquerReasons(ownedRegions);
        reasons.Remove(InvalidConquerReason.NotAdjacent);
        reasons.Remove(InvalidConquerReason.NotBorder);
        return reasons;
    }
}
