namespace Smallworld.Models.Races
{
    public class Triton : Race
    {
        public Triton() : base()
        {
            Name = "Tritons";
            StartingTokenCount = 6;
            MaxTokens = 11;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            if (region.IsAdjacentTo(RegionType.Sea) ||
                region.IsAdjacentTo(RegionType.Lake))
            {
                return 1;
            }
            return 0;
        }
    }
}