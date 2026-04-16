using System.Collections.Generic;

namespace Smallworld.Models.Powers;

public class Seafaring : Power
{
    public Seafaring()
    {
        Name = "Seafaring";
        StartingTokenCount = 5;
    }

    public override void FilterConquerReasons(List<InvalidConquerReason> reasons, List<Region> ownedRegions, Region region)
    {
        reasons.Remove(InvalidConquerReason.SeaOrLake);
    }
}
