using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Stout : Power
    {
        public Stout()
        {
            Name = "Stout";
            StartingTokenCount = 4;
        }

        public override void OnTurnEnd(List<Region> ownedRegions)
        {
            // prompt user whether to enter decline
            // if yes,
            // _racePower.EnterDecline();

            // only placeholder task for now
        }
    }
}