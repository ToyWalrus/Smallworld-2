namespace Smallworld.Models.Races
{
    public class Giant : Race
    {
        public Giant() : base()
        {
            Name = "Giants";
            StartingTokenCount = 6;
            MaxTokens = 11;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            foreach (Region adjacent in region.AdjacentTo)
            {
                if (adjacent.Type == RegionType.Mountain &&
                    _racePower.GetOwnedRegions().Contains(adjacent))
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}