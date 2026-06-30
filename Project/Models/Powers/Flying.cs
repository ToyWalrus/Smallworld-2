using System.Collections.Generic;

namespace Smallworld.Models.Powers;

public class Flying : Power
{
    public Flying()
    {
        Name = "Flying";
        StartingTokenCount = 5;
    }

    public override void ModifyConquerRestrictions(List<InvalidConquerReason> reasons, List<Region> ownedRegions, Region region)
    {
        reasons.Remove(InvalidConquerReason.NotAdjacent);
        reasons.Remove(InvalidConquerReason.NotBorder);
    }
}
