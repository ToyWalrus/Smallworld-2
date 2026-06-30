using System.Collections.Generic;

namespace Smallworld.Models.Races;

public class Halfling : Race
{
    private int totalRegionsConquered;
    private readonly List<Region> immuneRegions = new();

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
            region.SetImmune(true);
            immuneRegions.Add(region);
        }
        totalRegionsConquered++;
    }

    public override void ModifyConquerRestrictions(List<InvalidConquerReason> reasons, List<Region> ownedRegions, Region region)
    {
        if (ownedRegions.Count == 0)
        {
            reasons.Remove(InvalidConquerReason.NotBorder);
        }
    }

    public override void EnterDecline()
    {
        base.EnterDecline();
        foreach (var region in immuneRegions)
        {
            region.SetImmune(false);
        }
        immuneRegions.Clear();
    }
}
