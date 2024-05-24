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

        public override List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region)
        {
            var reasons = region.GetInvalidConquerReasons(ownedRegions);
            reasons.Remove(InvalidConquerReason.SeaOrLake);
            return reasons;
        }
    }
}