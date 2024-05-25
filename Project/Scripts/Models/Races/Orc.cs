using System.Collections.Generic;

namespace Smallworld.Models.Races;

public class Orc : Race
{
    private int nonEmptyRegionsConqueredThisTurn;

    public Orc() : base()
    {
        Name = "Orcs";
        StartingTokenCount = 5;
        MaxTokens = 10;
    }

    public override void OnTurnStart()
    {
        nonEmptyRegionsConqueredThisTurn = 0;
    }

    public override void OnRegionConquered(Region region)
    {
        if (region.IsOccupied)
        {
            nonEmptyRegionsConqueredThisTurn++;
        }
    }

    public override int TallyRaceBonusVP(List<Region> regions)
    {
        return nonEmptyRegionsConqueredThisTurn;
    }
}
