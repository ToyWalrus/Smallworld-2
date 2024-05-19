using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Powers
{
    public class Hill : Power
    {
        public Hill()
        {
            Name = "Hill";
            StartingTokenCount = 4;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            return regions.Count(region => region.Type == RegionType.Hill);
        }
    }
}