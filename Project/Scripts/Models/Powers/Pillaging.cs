using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Pillaging : Power
    {
        private int nonEmptyRegionsConqueredThisTurn;
        public Pillaging()
        {
            Name = "Pillaging";
            StartingTokenCount = 5;
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

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            return nonEmptyRegionsConqueredThisTurn;
        }
    }
}