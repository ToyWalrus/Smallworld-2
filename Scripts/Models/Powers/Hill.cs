using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    class Hill : Power
    {
        public Hill()
        {
            Name = "Hill";
            StartingTokenCount = 4;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            int hillsOwned = 0;
            foreach (Region region in regions)
            {
                if (region.Type == RegionType.Hill)
                {
                    hillsOwned++;
                }
            }
            return hillsOwned;
        }
    }
}