using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Bivouacking : Power
    {
        public Bivouacking()
        {
            Name = "Bivouacking";
            StartingTokenCount = 5;
        }

        public override void OnTurnEnd(List<Region> ownedRegions)
        {
            // prompt user where to put encampments            
        }
    }
}