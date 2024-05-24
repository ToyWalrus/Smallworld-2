namespace Smallworld.Models.Powers
{
    public class DragonMaster : Power
    {
        private bool hasUsedDragonTokenThisRound;
        private Region regionWithDragon;

        public DragonMaster()
        {
            Name = "Dragon master";
            StartingTokenCount = 5;
        }

        public override void OnTurnStart()
        {
            hasUsedDragonTokenThisRound = false;
            regionWithDragon?.RemoveAllTokensOfType(Token.Dragon);
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            if (hasUsedDragonTokenThisRound) return 0;

            // if using dragon token, only costs one.
            // prompt player whether to use dragon token

            hasUsedDragonTokenThisRound = true;
            region.AddToken(Token.Dragon);
            regionWithDragon = region;

            return 100; // just a big reduction number
        }
    }
}