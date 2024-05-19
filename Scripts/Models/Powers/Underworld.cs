namespace Smallworld.Models.Powers
{
    public class Underworld : Power
    {
        public Underworld()
        {
            Name = "Underworld";
            StartingTokenCount = 5;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            if (region.Attribute == RegionAttribute.Underworld ||
                region.SecondAttribute == RegionAttribute.Underworld)
            {
                return -1;
            }
            return 0;
        }
    }
}