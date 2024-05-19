using System.Collections.Generic;

namespace Smallworld.Models.Races
{
    public abstract class Race
    {
        public string Name { get; protected set; }
        public int MaxTokens { get; protected set; }
        public int StartingTokenCount { get; protected set; }
        public bool IsInDecline { get; protected set; }
        protected RacePower _racePower;
        public Race()
        {
            IsInDecline = false;
        }

        public void SetRacePower(RacePower data)
        {
            _racePower = data;
        }

        public virtual void OnTurnStart() { }
        public virtual void OnTurnEnd() { }
        public virtual void OnRegionConquered(Region region) { }
        public virtual int TallyRaceBonusVP(List<Region> regions) => 0;
        public virtual int GetRegionConquerCostReduction(Region region) => 0;
        public virtual void EnterDecline()
        {
            IsInDecline = true;
        }
    }
}