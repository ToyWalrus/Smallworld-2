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
            return region.HasAttribute(RegionAttribute.Underworld) ? 1 : 0;
        }
    }
}