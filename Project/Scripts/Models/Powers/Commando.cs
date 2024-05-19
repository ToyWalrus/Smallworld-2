namespace Smallworld.Models.Powers
{
    public class Commando : Power
    {
        public Commando()
        {
            Name = "Commando";
            StartingTokenCount = 4;
        }

        public override int GetRegionConquerCostReduction(Region region)
        {
            return 1;
        }
    }
}