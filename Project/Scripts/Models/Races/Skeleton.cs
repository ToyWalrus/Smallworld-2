using System.Collections.Generic;

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

        public override int GetTroopRedeploymentCount(List<Region> ownedRegions)
        {
            // Get extra troops based on nonEmptyRegionsConqueredThisTurn / 2
            // but no more than MaxTokens
            var baseRedeploymentCount = base.GetTroopRedeploymentCount(ownedRegions);
            return System.Math.Min(MaxTokens, baseRedeploymentCount + (int)System.Math.Floor(nonEmptyRegionsConqueredThisTurn / 2.0));
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