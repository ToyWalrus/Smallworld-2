using Smallworld.Models;

namespace Smallworld.Events;

public class PlayerSelectEvent : IEvent
{
    public string Name => "RacePower selected";
    public Player Player { get; }

    public PlayerSelectEvent(Player player)
    {
        Player = player;
    }
}