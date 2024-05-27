using System.Collections.Generic;
using System.Linq;
using Smallworld.Models;

namespace Smallworld.Logic;

public class GamePlayer
{
    public Player Player { get; }
    public string Name => Player.Name;
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
}