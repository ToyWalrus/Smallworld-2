using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smallworld.Models.Powers;
using Smallworld.Models.Races;
using Math = System.Math;

namespace Smallworld.Models;

public class RacePower
{
    public string Name => $"The {Power.Name} {Race.Name}";

    public Player Owner { get; private set; }
    public Race Race { get; private set; }
    public Power Power { get; private set; }
    public int AvailableTokenCount { get; private set; }
    public bool IsInDecline { get => Race.IsInDecline && Power.IsInDecline; }

    private readonly List<Region> ownedRegions;

    public RacePower(Race race, Power power)
    {
        Race = race;
        Power = power;
        AvailableTokenCount = Race.StartingTokenCount + Power.StartingTokenCount;
        ownedRegions = new();

        Power.SetRacePower(this);
    }

    public void SetOwner(Player owner)
    {
        Owner = owner;
    }

    public bool CanEnterDecline()
    {
        return Race.CanEnterDecline() || Power.CanEnterDecline();
    }

    public void OnTurnStart()
    {
        Race.OnTurnStart();
        Power.OnTurnStart();
    }

    public async Task OnTurnEnd()
    {
        Race.OnTurnEnd();
        await Power.OnTurnEnd();
    }

    public void ModifyConquerRestrictions(List<InvalidConquerReason> reasons, Region region)
    {
        Race.ModifyConquerRestrictions(reasons, ownedRegions, region);
        Power.ModifyConquerRestrictions(reasons, ownedRegions, region);
    }

    public void ModifyDefenseRestrictions(List<InvalidConquerReason> reasons, RacePower attacker, Region region)
    {
        Race.ModifyDefenseRestrictions(reasons, attacker, region);
        Power.ModifyDefenseRestrictions(reasons, attacker, region);
    }

    public void SpendToken(int count)
    {
        AvailableTokenCount -= count;
    }

    public void ConquerRegion(Region region, int cost)
    {
        region.WasConquered(this, cost);

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

    public int TallyVP()
    {
        int raceVP = Race.TallyRaceBonusVP(ownedRegions);
        int powerVP = Power.TallyPowerBonusVP(ownedRegions);
        return raceVP + powerVP + ownedRegions.Count;
    }

    public int EstimateRegionConquerCost(Region region)
    {
        int raceCostReduction = Race.GetRegionConquerCostReduction(region);
        int powerCostReduction = Power.GetEstimatedConquerCostReduction(region);
        return Math.Max(1, region.GetBaseConquerCost() - raceCostReduction - powerCostReduction);
    }

    public async Task<int> GetFinalRegionConquerCost(Region region)
    {
        int raceCostReduction = Race.GetRegionConquerCostReduction(region);
        int powerCostReduction = await Power.GetRegionConquerCostReduction(region);
        return Math.Max(1, region.GetBaseConquerCost() - raceCostReduction - powerCostReduction);
    }

    public void EnterDecline()
    {
        Race.EnterDecline();
        Power.EnterDecline();

        // Does this belong here...?
        if (IsInDecline)
        {
            AvailableTokenCount = 0;
            foreach (var region in ownedRegions)
            {
                region.ClearExcessRaceTokens();
            }
        }
    }

    public void AbandonAllRegions()
    {
        foreach (var region in ownedRegions)
        {
            region.Abandon();
        }
        ownedRegions.Clear();
    }

    public List<Region> GetOwnedRegions() => new(ownedRegions);

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
