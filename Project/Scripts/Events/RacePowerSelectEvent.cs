using Smallworld.Models;

namespace Smallworld.Events;

public class RacePowerSelectEvent : IEvent
{
    public string Name => "RacePower selected";
    public RacePower RacePower { get; }
    public Player OwningPlayer => RacePower.Owner;

    public RacePowerSelectEvent(RacePower racePower)
    {
        RacePower = racePower;
    }

    public override string ToString() => $"{Name} ({RacePower.Name})";
}