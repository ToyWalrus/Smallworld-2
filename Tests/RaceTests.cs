using Smallworld.Models.Races;
using Smallworld.Models;

namespace Tests
{
    [TestClass]
    public class RaceTests
    {
        [TestMethod]
        public void Dwarf_TallyRaceBonusVP_ReturnsNumberOfMinesOwned()
        {
            var dwarf = new Dwarf();
            var minesOwned = 10;
            var regions = new List<Region>();

            for (int i = 0; i < minesOwned; i++)
            {
                regions.Add(new Region(RegionType.Farmland, RegionAttribute.Mine, false));
            }

            Assert.AreEqual(dwarf.TallyRaceBonusVP(regions), minesOwned);
        }
    }
} 