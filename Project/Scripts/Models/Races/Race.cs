using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Races
{
    public abstract class Race
    {
        public string Name { get; protected set; }
        public int MaxTokens { get; protected set; }
        public int StartingTokenCount { get; protected set; }
        public bool IsInDecline { get; protected set; }
        // protected RacePower _racePower;

        public Race()
        {
            IsInDecline = false;
        }

        public void SetRacePower(RacePower data)
        {
            // _racePower = data;
        }

        public virtual void OnTurnStart() { }
        public virtual void OnTurnEnd() { }
        public virtual void OnRegionConquered(Region region) { }
        public virtual int GetRegionConquerCostReduction(Region region) => 0;
        public virtual int TallyRaceBonusVP(List<Region> ownedRegions) => 0;
        public virtual int GetTroopRedeploymentCount(List<Region> ownedRegions)
        {
            var totalRaceTokens = ownedRegions.Sum(r => r.NumRaceTokens);

            // Need to leave at least one token in each region
            return System.Math.Max(0, totalRaceTokens - ownedRegions.Count);
        }
        public virtual void EnterDecline()
        {
            IsInDecline = true;
        }
    }
}