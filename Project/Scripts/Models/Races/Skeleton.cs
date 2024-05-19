namespace Smallworld.Models.Races
{
    public class Skeleton : Race
    {
        private int nonEmptyRegionsConqueredThisTurn;

        public Skeleton() : base()
        {
            Name = "Skeletons";
            StartingTokenCount = 6;
            MaxTokens = 20;
        }

        public override void OnTurnStart()
        {
            nonEmptyRegionsConqueredThisTurn = 0;
        }

        public override void OnTurnEnd()
        {
            // get extra troops based on nonEmptyRegionsConqueredThisTurn / 2
            // but no more than MaxTokens
        }

        public override void OnRegionConquered(Region region)
        {
            if (region.IsOccupied)
            {
                nonEmptyRegionsConqueredThisTurn++;
            }
        }

    }
}