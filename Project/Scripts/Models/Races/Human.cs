using System.Collections.Generic;
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
            int total = 0;
            foreach (Region region in regions)
            {
                if (region.Type == RegionType.Farmland)
                {
                    total++;
                }
            }
            return total;
        }
    }
}