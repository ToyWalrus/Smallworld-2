using Smallworld.Models;

namespace Smallworld.Events;

public class RacePowerSelectEvent : IEvent
{
    public string Name => "RacePower selected";
    public RacePower RacePower { get; }
    public Player OwningPlayer { get; }

    public RacePowerSelectEvent(RacePower racePower, Player owner)
    {
        RacePower = racePower;
        OwningPlayer = owner;
    }

    public override string ToString() => $"{Name} ({RacePower.Name})";
}