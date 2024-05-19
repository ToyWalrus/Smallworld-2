namespace Smallworld.Models.Powers
{
    public class Berserk : Power
    {
        public Berserk()
        {
            Name = "Berserk";
            StartingTokenCount = 4;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            // roll die
            return 0;
        }
    }
}