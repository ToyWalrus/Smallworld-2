using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Races
{
    public class Human : Race
    {
        public Human() : base()
        {
            Name = "Humans";
            StartingTokenCount = 5;
            MaxTokens = 10;
        }

        public override int TallyRaceBonusVP(List<Region> regions)
        {
            if (IsInDecline) return 0;
            return regions.Count(region => region.Type == RegionType.Farmland);
        }
    }
}