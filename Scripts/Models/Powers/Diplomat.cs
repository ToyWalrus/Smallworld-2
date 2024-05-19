using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers
{
    class Diplomat : Power
    {
        private List<RacePower> racesAttacked;
        public Diplomat()
        {
            Name = "Diplomatic";
            StartingTokenCount = 5;
            racesAttacked = new List<RacePower>();
        }

        public override Task OnRegionConquered(Region region)
        {
            if (region.IsOccupiedByOpponent)
            {
                racesAttacked.Add(region.OccupiedBy);
            }
            return Task.CompletedTask;
        }

        public override Task OnTurnEnd(List<Region> ownedRegions)
        {
            // prompt player to choose one he didn't attack
            return Task.CompletedTask; // placeholder for prompt
        }
    }
}