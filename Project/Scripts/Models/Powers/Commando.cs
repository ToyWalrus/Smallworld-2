using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class Commando : Power
{
    public Commando()
    {
        Name = "Commando";
        StartingTokenCount = 4;
    }

    public override Task<int> GetRegionConquerCostReduction(Region region)
    {
        return Task.FromResult(1);
    }
}
