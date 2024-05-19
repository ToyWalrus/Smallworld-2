namespace Smallworld.Models.Powers
{
    public class Mounted : Power
    {
        public Mounted()
        {
            Name = "Mounted";
            StartingTokenCount = 5;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            if (region.Type == RegionType.Hill || region.Type == RegionType.Farmland)
            {
                return 1;
            }
            return 0;
        }
    }
}