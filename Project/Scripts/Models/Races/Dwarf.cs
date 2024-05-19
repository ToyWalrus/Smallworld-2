using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Races
{
    public class Dwarf : Race
    {
        public Dwarf() : base()
        {
            Name = "Dwarves";
            StartingTokenCount = 3;
            MaxTokens = 8;
        }

        public override int TallyRaceBonusVP(List<Region> regions)
        {
            return regions.Count(region => region.HasAttribute(RegionAttribute.Mine));
        }
    }
}