using System.Collections.Generic;

namespace Smallworld.Models.Powers;

public class Diplomat : Power
{
    private readonly List<RacePower> _racesAttacked;

    public Diplomat()
    {
        Name = "Diplomatic";
        StartingTokenCount = 5;
        _racesAttacked = new List<RacePower>();
    }

    public override void OnTurnStart()
    {
        _racesAttacked.Clear();
    }

    public override void OnRegionConquered(Region region)
    {
        if (region.IsOccupied)
        {
            _racesAttacked.Add(region.OccupiedBy);
        }

    }

    public override void OnTurnEnd()
    {
        // prompt player to choose one he didn't attack
    }
}
