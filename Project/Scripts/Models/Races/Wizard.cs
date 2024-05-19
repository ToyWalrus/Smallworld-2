using System.Collections.Generic;

namespace Smallworld.Models.Races
{
    public class Wizard : Race
    {
        public Wizard() : base()
        {
            Name = "Wizards";
            StartingTokenCount = 5;
            MaxTokens = 10;
        }

        public override int TallyRaceBonusVP(List<Region> regions)
        {
            if (IsInDecline) return 0;
            int total = 0;
            foreach (Region region in regions)
            {
                if (region.Attribute == RegionAttribute.Magic ||
                    region.SecondAttribute == RegionAttribute.Magic)
                {
                    total++;
                }
            }
            return total;
        }
    }
}