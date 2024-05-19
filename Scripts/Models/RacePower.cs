using System.Collections.Generic;
using Smallworld.Models.Races;
using Smallworld.Models.Powers;

using Math = System.Math;

namespace Smallworld.Models
{
    public class RacePower
    {
        public string Name => $"The {Power.Name} {Race.Name}";

        public Race Race { get; private set; }
        public Power Power { get; private set; }
        public int AvailableTokenCount { get; private set; }
        public bool IsInDecline { get => Race.IsInDecline; }
        private List<Region> ownedRegions;

        public RacePower(Race race, Power power)
        {
            Race = race;
            Power = power;
            Setup();
        }

        private void Setup()
        {
            Race.SetRacePower(this);
            Power.SetRacePower(this);
            AvailableTokenCount = Race.StartingTokenCount + Power.StartingTokenCount;
            ownedRegions = new List<Region>();
        }

        public void OnTurnStart()
        {
            Race.OnTurnStart();
            Power.OnTurnStart();
        }

        public void OnTurnEnd()
        {
            Race.OnTurnEnd();
            Power.OnTurnEnd(ownedRegions);
        }

        public void OnNewRegionConquered(Region region, int cost)
        {
            Race.OnRegionConquered(region);
            Power.OnRegionConquered(region);
            AvailableTokenCount = Math.Max(0, AvailableTokenCount - cost);
            ownedRegions.Add(region);
        }

        public void OnWasConquered(Region region, int troopReimbursement)
        {
            if (!IsInDecline || Race is Ghoul)
            {
                AvailableTokenCount += troopReimbursement;
            }
            ownedRegions.Remove(region);
        }

        public int TallyBonusVP()
        {
            int raceVP = Race.TallyRaceBonusVP(ownedRegions);
            int powerVP = Power.TallyPowerBonusVP(ownedRegions);
            return raceVP + powerVP;
        }

        public int GetFinalRegionConquerCost(Region region)
        {
            int raceCostReduction = Race.GetRegionConquerCostReduction(region);
            int powerCostReduction = Power.GetRegionConquerCostReduction(region);
            // race and power cost reductions will be either 0 or negative, so we need to add instead of subtract
            return Math.Max(1, region.GetBaseConquerCost() + raceCostReduction + powerCostReduction);
        }

        public void EnterDecline()
        {
            Race.EnterDecline();
        }

        public bool IsValidConquerRegion(Region region, bool isFirstConquest, out string reason)
        {
            reason = null;
            if (region.IsImmune())
            {
                reason = "Region immune";
                return false;
            }

            bool regionIsSeaOrLake = region.Type == RegionType.Sea || region.Type == RegionType.Lake;
            bool isSeafaring = Power is Seafaring;
            bool isRegionAdjacent = IsRegionAdjacent(region);

            if (Power is Flying && !regionIsSeaOrLake)
            {
                return true;
            }

            if (isFirstConquest)
            {
                if (Race is Halfling && (!regionIsSeaOrLake || isSeafaring))
                {
                    return true;
                }
                if (region.IsBorder && (!regionIsSeaOrLake || isSeafaring))
                {
                    return true;
                }
            }

            if (regionIsSeaOrLake && isSeafaring && isRegionAdjacent)
            {
                return true;
            }

            if (!isRegionAdjacent)
            {
                reason = "Non-adjacent region";
            }
            if (regionIsSeaOrLake)
            {
                reason += " or is sea or lake";
            }
            return isRegionAdjacent && !regionIsSeaOrLake;
        }

        public List<Region> GetOwnedRegions() => new(ownedRegions);

        private bool IsRegionAdjacent(Region region)
        {
            bool hasUnderworldAccess = false;

            foreach (Region owned in ownedRegions)
            {
                if (owned.AdjacentTo.Contains(region))
                {
                    return true;
                }
                if (owned.Attribute == RegionAttribute.Underworld ||
                    owned.SecondAttribute == RegionAttribute.Underworld)
                {
                    hasUnderworldAccess = true;
                }
            }

            return Power is Underworld &&
                hasUnderworldAccess &&
                (region.Attribute == RegionAttribute.Underworld ||
                region.SecondAttribute == RegionAttribute.Underworld);
        }

        public bool HasEnoughTokensToConquer(Region region)
        {
            return AvailableTokenCount >= GetFinalRegionConquerCost(region);
        }

        public static bool operator ==(RacePower left, RacePower right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(RacePower left, RacePower right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is RacePower other)
            {
                return Name == other.Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}