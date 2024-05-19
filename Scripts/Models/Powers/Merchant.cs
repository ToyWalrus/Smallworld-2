namespace Smallworld.Models.Powers
{
    public class Merchant : Power
    {
        public Merchant()
        {
            Name = "Merchant";
            StartingTokenCount = 2;
        }

        public override int TallyPowerBonusVP(List<Region> regions)
        {
            // 1 bonus VP for each region occupied
            return regions.Count;
        }
    }
}