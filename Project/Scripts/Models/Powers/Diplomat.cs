using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Diplomat : Power
    {
        private List<RacePower> _racesAttacked;
        public Diplomat()
        {
            Name = "Diplomatic";
            StartingTokenCount = 5;
            _racesAttacked = new List<RacePower>();
        }

        public override void OnRegionConquered(Region region)
        {
            if (region.IsOccupied && region.OccupiedBy != _racePower)
            {
                _racesAttacked.Add(region.OccupiedBy);
            }

        }

        public override void OnTurnEnd(List<Region> ownedRegions)
        {
            // prompt player to choose one he didn't attack

        }
    }
}