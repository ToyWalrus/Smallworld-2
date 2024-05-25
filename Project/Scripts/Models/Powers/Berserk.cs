using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class Berserk : Power
{
    public Berserk()
    {
        Name = "Berserk";
        StartingTokenCount = 4;
    }

    public override Task<int> GetRegionConquerCostReduction(Region region)
    {
        return DiceRoller.RollDiceAsync(CustomProbabilityDice.Reinforcement);
    }
}
