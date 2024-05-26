using System.Collections.Generic;
using Smallworld.Models.Races;
using Smallworld.Models.Powers;
using System.Linq;

using Math = System.Math;
using System.Threading.Tasks;

namespace Smallworld.Models;

public class RacePower
{
    public string Name => $"The {Power.Name} {Race.Name}";

    public Race Race { get; private set; }
    public Power Power { get; private set; }
    public int AvailableTokenCount { get; private set; }
    public bool IsInDecline { get => Race.IsInDecline; }

    private readonly List<Region> ownedRegions;

    public RacePower(Race race, Power power)
    {
        Race = race;
        Power = power;
        AvailableTokenCount = Race.StartingTokenCount + Power.StartingTokenCount;
        ownedRegions = new();

        Power.SetRacePower(this);
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

    public void OnNewRegionConquered(Region region, int cost)
    {
        Race.OnRegionConquered(region);
        Power.OnRegionConquered(region);
        AvailableTokenCount = Math.Max(0, AvailableTokenCount - cost);
        ownedRegions.Add(region);
    }

    public void OnWasConquered(Region region, int troopReimbursement)
    {
        // TODO: should find a way to not have to check for Ghoul here?
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

        if (Race.IsInDecline && Power.IsInDecline)
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

    public (bool, string) IsValidConquerRegion(Region region)
    {

        if (region.OccupiedBy == this)
        {
            return (false, "Region is already occupied by you");
        }

        var invalidRaceConquerReasons = Race.GetInvalidConquerReasons(ownedRegions, region);
        var invalidPowerConquerReasons = Power.GetInvalidConquerReasons(ownedRegions, region);

        // If there are no invalid reasons, for either the race or power to conquer, then the conquest is valid
        if (
            !invalidRaceConquerReasons.Any() ||
            !invalidPowerConquerReasons.Any()
        )
        {
            return (true, "");
        }

        var reasons = new HashSet<InvalidConquerReason>(invalidRaceConquerReasons.Concat(invalidPowerConquerReasons));

        string reason = "| ";
        foreach (var currentReason in reasons)
        {
            switch (currentReason)
            {
                case InvalidConquerReason.NotAdjacent:
                    reason += "Region is not adjacent to any of your regions | ";
                    break;
                case InvalidConquerReason.SeaOrLake:
                    reason += "Region is a sea or lake | ";
                    break;
                case InvalidConquerReason.NotBorder:
                    reason += "First conquest must happen on a border region | ";
                    break;
                case InvalidConquerReason.RegionImmune:
                    reason += "Region is immune to conquest | ";
                    break;
            }
        }

        return (false, reason.Trim());
    }

    public List<Region> GetOwnedRegions() => new(ownedRegions);

    public async Task<bool> HasEnoughTokensToConquer(Region region)
    {
        return AvailableTokenCount >= await GetFinalRegionConquerCost(region);
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
