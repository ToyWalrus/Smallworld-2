using System.Collections.Generic;

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
            int totalMines = 0;
            foreach (Region region in regions)
            {
                if (region.HasAttribute(RegionAttribute.Mine))
                {
                    totalMines++;
                }
            }
            return totalMines;
        }
    }
}