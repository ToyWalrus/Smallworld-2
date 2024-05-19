using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers
{
    public abstract class Power
    {
        public String Name { get; protected set; }
        public int StartingTokenCount { get; protected set; }
        protected RacePower _racePower;

        public void SetRacePower(RacePower rp)
        {
            _racePower = rp;
        }

        public virtual int TallyPowerBonusVP(List<Region> regions)
        {
            return 0;
        }

        public virtual int GetRegionConquerCostReduction(Region region)
        {
            return 0;
        }

        public virtual void OnTurnStart() { }
        public virtual Task OnTurnEnd(List<Region> ownedRegions) => Task.CompletedTask;

        /// <summary>
        /// This method should be called before moving around
        /// any troops to and from regions.
        /// </summary>
        /// <param name="region">the conquered region.</param>
        public virtual Task OnRegionConquered(Region region) => Task.CompletedTask;

        public override String ToString() => Name;
    }
}