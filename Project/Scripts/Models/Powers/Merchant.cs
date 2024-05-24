using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public class Merchant : Power
    {
        public Merchant()
        {
            Name = "Merchant";
            StartingTokenCount = 2;
        }

        public override int TallyPowerBonusVP(List<Region> ownedRegions)
        {
            // 1 bonus VP for each region occupied
            return IsInDecline ? 0 : ownedRegions.Count;
        }
    }
}