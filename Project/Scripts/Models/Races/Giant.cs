using System.Linq;

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
            return region.AdjacentTo.Exists(adjacent => adjacent.Type == RegionType.Mountain) ? 1 : 0;
        }
    }
}