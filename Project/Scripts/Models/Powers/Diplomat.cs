using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class Diplomat : Power
{
    private readonly HashSet<RacePower> racesAttacked;
    private RacePower protectedRacePower;

    public Diplomat()
    {
        Name = "Diplomatic";
        StartingTokenCount = 5;
        racesAttacked = new();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        racesAttacked.Clear();
        protectedRacePower = null;
    }

    public override void OnRegionConquered(Region region)
    {
        base.OnRegionConquered(region);
        if (region.OccupiedBy != null)
        {
            racesAttacked.Add(region.OccupiedBy);
        }
    }

    public override async Task OnTurnEnd()
    {
        if (GameRef == null) return;

        // Eligible players: opponents whose active race was not attacked this turn
        var eligiblePlayers = GameRef.Players
            .Where(p => p != racePower.Owner && p.HasActiveRace && !racesAttacked.Contains(p.ActiveRacePower))
            .ToList();

        if (eligiblePlayers.Count == 0) return;

        var selectedPlayer = await PlayerSelection.SelectAsync(eligiblePlayers, (player) => $"You attacked {player.Name} this turn");
        protectedRacePower = selectedPlayer?.ActiveRacePower;
    }

    public override void ModifyDefenseRestrictions(List<InvalidConquerReason> reasons, RacePower attacker, Region region)
    {
        if (protectedRacePower != null && attacker == protectedRacePower && !attacker.IsInDecline)
        {
            reasons.Add(InvalidConquerReason.ProtectedByDiplomat);
        }
    }
}
