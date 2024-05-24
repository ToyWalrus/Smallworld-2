using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
    public abstract class Power
    {
        public string Name { get; protected set; }
        public int StartingTokenCount { get; protected set; }
        public bool IsInDecline { get; protected set; }
        protected RacePower _racePower;

        public Power()
        {
            IsInDecline = false;
        }

        public void SetRacePower(RacePower rp)
        {
            _racePower = rp;
        }

        public virtual void OnTurnStart() { }
        public virtual void OnTurnEnd(List<Region> ownedRegions) { }
        public virtual int TallyPowerBonusVP(List<Region> regions) => 0;
        public virtual int GetRegionConquerCostReduction(Region region) => 0;

        /// <summary>
        /// This method should be called before moving around
        /// any troops to and from regions.
        /// </summary>
        /// <param name="region">the conquered region.</param>
        public virtual void OnRegionConquered(Region region) { }

        public virtual void EnterDecline()
        {
            IsInDecline = true;
        }

        public override string ToString() => Name;
    }
}