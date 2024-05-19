using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Heroic : Power
    {
        public Heroic()
        {
            Name = "Heroic";
            StartingTokenCount = 5;
        }

        public override void OnTurnEnd(List<Region> ownedRegions)
        {
            // place heroic tokens on two regions
            // prompt player to pick two regions            
        }
    }
}