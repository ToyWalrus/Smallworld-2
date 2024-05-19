using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Fortified : Power
    {
        private int numFortsBuilt;
        private const int MAX_FORTS = 6;

        public Fortified()
        {
            Name = "Fortified";
            StartingTokenCount = 3;
            numFortsBuilt = 0;
        }

        public override void OnTurnEnd(List<Region> ownedRegions)
        {
            if (numFortsBuilt == MAX_FORTS) return;
            // place 1 new fort in region with no forts
            // prompt player to pick region
            // if fort was placed,
            //  numFortsBuilt += 1;

            // placeholder for prompt task
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            if (_racePower.IsInDecline) return 0;
            return numFortsBuilt;
        }
    }
}