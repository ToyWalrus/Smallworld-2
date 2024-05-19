using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    class Swamp : Power
    {
        public Swamp()
        {
            Name = "Swamp";
            StartingTokenCount = 4;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            int swampsOwned = 0;
            foreach (Region region in regions)
            {
                if (region.Type == RegionType.Swamp)
                {
                    swampsOwned++;
                }
            }
            return swampsOwned;
        }
    }
}