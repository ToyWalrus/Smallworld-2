using System.Collections.Generic;
using System.Linq;
using Smallworld.Models;

namespace Smallworld.Logic;

public class GamePlayer
{
    public Player Player { get; }
    public string Name => Player.Name;
    public int Score => Player.Score;
    public int AvailableTokens => ActiveRacePowers.FirstOrDefault()?.AvailableTokenCount ?? 0;
    public bool DidEnterDeclineLastTurn { get; set; }
    public IEnumerable<RacePower> ActiveRacePowers => Player.RacePowers.Where(rp => !rp.IsInDecline);

    public GamePlayer(Player player)
    {
        Player = player;
    }

    public void AddRacePower(RacePower power)
    {
        Player.AddRacePower(power);
    }

    public void EnterDecline()
    {
        Player.EnterDecline();
        DidEnterDeclineLastTurn = true;
    }

    public void TallyVP()
    {
        Player.AddScore(Player.TallyVP());
    }

    public bool CanConquerRegion(Region region)
    {
        return ActiveRacePowers.Any(rp => rp.IsValidConquerRegion(region).Item1);
    }

    public string GetCannotConquerReason(Region region)
    {
        foreach (var rp in ActiveRacePowers)
        {
            var (canConquer, reason) = rp.IsValidConquerRegion(region);
            if (!canConquer)
            {
                return reason;
            }
        }

        return null;
    }
}