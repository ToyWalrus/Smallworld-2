using System.Collections.Generic;
using System.Threading.Tasks;

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

    public override Task OnTurnEnd()
    {
        // TODO: Need to have a list of all players, then remove the races attacked from the list
        // var selected = await PlayerSelection.SelectAsync(_racesAttacked);

        // TODO: Also need a way to enforce the "no attacking" rule

        return Task.CompletedTask;
    }
}
