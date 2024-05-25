using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class DragonMaster : Power
{
    private bool hasUsedDragonTokenThisRound;
    private Region regionWithDragon;

    public DragonMaster()
    {
        Name = "Dragon master";
        StartingTokenCount = 5;
    }

    public override void OnTurnStart()
    {
        hasUsedDragonTokenThisRound = false;
        regionWithDragon?.RemoveAllTokensOfType(Token.Dragon);
    }

    public async override Task<int> GetRegionConquerCostReduction(Region region)
    {
        if (hasUsedDragonTokenThisRound) return 0;

        var confirmed = await Confirmation.ConfirmAsync("Do you want to use the dragon token to conquer this region?");
        if (!confirmed) return 0;

        hasUsedDragonTokenThisRound = true;
        region.AddToken(Token.Dragon);
        regionWithDragon = region;

        return int.MaxValue;
    }
}
