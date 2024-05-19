using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers
{
    class Pillaging : Power
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

        public override Task OnRegionConquered(Region region)
        {
            if (region.IsOccupied)
            {
                nonEmptyRegionsConqueredThisTurn++;
            }
            return Task.CompletedTask;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            return nonEmptyRegionsConqueredThisTurn;
        }
    }
}