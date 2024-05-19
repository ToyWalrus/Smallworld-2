using System.Threading.Tasks;

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

        public override Task OnTurnEnd()
        {
            // get extra troops based on nonEmptyRegionsConqueredThisTurn / 2
            // but no more than MaxTokens
            return Task.CompletedTask;
        }

        public override Task OnRegionConquered(Region region)
        {
            if (region.IsOccupied)
            {
                nonEmptyRegionsConqueredThisTurn++;
            }
            return Task.CompletedTask;
        }

    }
}