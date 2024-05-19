using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    class DragonMaster : Power
    {
        private bool hasUsedDragonTokenThisRound;
        public DragonMaster()
        {
            Name = "Dragon master";
            StartingTokenCount = 5;
        }

        public override void OnTurnStart()
        {
            hasUsedDragonTokenThisRound = false;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            if (!hasUsedDragonTokenThisRound)
            {
                // if using dragon token, only costs one.
                // prompt player whether to use dragon token

                // need to take dragon token from previous region too
                region.AddToken(Token.Dragon);
                hasUsedDragonTokenThisRound = true;
                return -10;

                // if didn't use dragon token, return 0
            }
            return 0;
        }
    }
}