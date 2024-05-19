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

    /*
     * using Smallworld.Models;
using Smallworld.Models.Races;
using Smallworld.Models.Powers;

namespace Tests;

public class RaceTests
{


    [Fact]
    public void Dwarf_TallyRaceBonusVP_ReturnsNumberOfMinesOwned()
    {
        var dwarf = new Dwarf();
        var minesOwned = 10;
        var regions = new List<Region>();

        for (int i = 0; i < minesOwned; i++)
        {
            regions.Add(new Region(RegionType.Farmland, RegionAttribute.Mine, false));
        }

        Assert.Equal(dwarf.TallyRaceBonusVP(regions), minesOwned);
    }
}
    */
}