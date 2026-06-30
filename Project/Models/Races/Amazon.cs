namespace Smallworld.Models.Races;

public class Amazon : Race
{
    public Amazon() : base()
    {
        Name = "Amazons";
        StartingTokenCount = 6;
        MaxTokens = 15;
    }

    public override int GetRegionConquerCostReduction(Region region)
    {
        // TODO: only gets 4 extra tokens for purposes of conquering, not redeploying
        return 4;
    }
}
