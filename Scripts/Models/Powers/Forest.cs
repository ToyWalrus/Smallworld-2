using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    class Forest : Power
    {
        public Forest()
        {
            Name = "Forest";
            StartingTokenCount = 4;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            int forestsOwned = 0;
            foreach (Region region in regions)
            {
                if (region.Type == RegionType.Forest)
                {
                    forestsOwned++;
                }
            }
            return forestsOwned;
        }
    }
}