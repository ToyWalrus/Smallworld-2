using System.Collections.Generic;
using System.Threading.Tasks;
namespace Smallworld.Models.Races
{
    public class Orc : Race
    {
        private int nonEmptyRegionsConqueredThisTurn;

        public Orc() : base()
        {
            Name = "Orcs";
            StartingTokenCount = 5;
            MaxTokens = 10;
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

        public override int TallyRaceBonusVP(List<Region> regions)
        {
            return nonEmptyRegionsConqueredThisTurn;
        }
    }
}