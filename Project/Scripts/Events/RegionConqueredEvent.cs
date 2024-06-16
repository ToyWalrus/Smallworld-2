using Smallworld.Models;

namespace Smallworld.Events;

public class RegionConqueredEvent:IEvent
{
    public string Name => "Region conquered";
    public Region Region { get; private set; }
    public Player Conqueror => Region.OccupiedBy.Owner;

    public RegionConqueredEvent(Region region)
    {
        Region = region;
    }
}
