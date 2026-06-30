using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class Mounted : Power
{
    public Mounted()
    {
        Name = "Mounted";
        StartingTokenCount = 5;
    }

    public override Task<int> GetRegionConquerCostReduction(Region region)
    {
        if (region.Type == RegionType.Hill || region.Type == RegionType.Farmland)
        {
            return Task.FromResult(1);
        }
        return Task.FromResult(0);
    }
}
