using System.Collections.Generic;

namespace Smallworld.Models.Races;

public class Halfling : Race
{
    private int totalRegionsConquered;

    public Halfling() : base()
    {
        Name = "Halflings";
        StartingTokenCount = 6;
        MaxTokens = 11;
        totalRegionsConquered = 0;
    }

    public override void OnRegionConquered(Region region)
    {
        base.OnRegionConquered(region);
        if (totalRegionsConquered < 2)
        {
            region.AddToken(Token.HoleInTheGround);
        }
        totalRegionsConquered++;
    }

    public override void FilterConquerReasons(List<InvalidConquerReason> reasons, List<Region> ownedRegions, Region region)
    {
        if (ownedRegions.Count == 0)
        {
            reasons.Remove(InvalidConquerReason.NotBorder);
        }
    }
}
