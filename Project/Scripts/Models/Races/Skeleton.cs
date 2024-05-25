using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Races;

public class Skeleton : Race
{
    private int nonEmptyRegionsConqueredThisTurn;

    public Skeleton() : base()
    {
        Name = "Skeletons";
        StartingTokenCount = 6;
        MaxTokens = 20;
    }

    public override void OnTurnStart()
    {
        nonEmptyRegionsConqueredThisTurn = 0;
    }

    public override List<Token> GetRedeploymentTokens(List<Region> ownedRegions)
    {
        // Get extra troops based on nonEmptyRegionsConqueredThisTurn / 2
        // but no more than MaxTokens
        var baseRedeployment = base.GetRedeploymentTokens(ownedRegions);
        var newCount = System.Math.Min(MaxTokens, baseRedeployment.Count + (int)System.Math.Floor(nonEmptyRegionsConqueredThisTurn / 2.0));

        return Enumerable.Repeat(Token.Race, newCount).ToList();
    }

    public override void OnRegionConquered(Region region)
    {
        if (region.IsOccupied)
        {
            nonEmptyRegionsConqueredThisTurn++;
        }
    }

}
