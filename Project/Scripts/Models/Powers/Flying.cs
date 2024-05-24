using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Flying : Power
    {
        public Flying()
        {
            Name = "Flying";
            StartingTokenCount = 5;
        }

        public override List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region, bool isFirstConquest)
        {
            var reasons = region.GetInvalidConquerReasons(ownedRegions, isFirstConquest);
            reasons.Remove(InvalidConquerReason.NotAdjacent);
            reasons.Remove(InvalidConquerReason.NotBorder);
            return reasons;
        }
    }
}