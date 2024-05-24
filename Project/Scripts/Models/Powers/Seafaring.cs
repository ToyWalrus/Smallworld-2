using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Seafaring : Power
    {
        public Seafaring()
        {
            Name = "Seafaring";
            StartingTokenCount = 5;
        }

        public override List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region, bool isFirstConquest)
        {
            var reasons = region.GetInvalidConquerReasons(ownedRegions, isFirstConquest);
            reasons.Remove(InvalidConquerReason.SeaOrLake);
            return reasons;
        }
    }
}