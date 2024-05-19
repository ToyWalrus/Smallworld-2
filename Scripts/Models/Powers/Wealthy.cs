using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    class Wealthy : Power
    {
        private bool haveCollectedBonus;
        private const int BONUS_AMOUNT = 7;
        public Wealthy()
        {
            Name = "Wealthy";
            StartingTokenCount = 4;
            haveCollectedBonus = false;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            if (!haveCollectedBonus)
            {
                haveCollectedBonus = true;
                return BONUS_AMOUNT;
            }
            return 0;
        }
    }
}