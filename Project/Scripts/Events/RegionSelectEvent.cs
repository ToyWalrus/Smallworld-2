using Smallworld.Models;

namespace Smallworld.Events;

public class RegionSelectEvent : IEvent
{
    public string Name => "Region selected";
    public Region Region { get; }

    public RegionSelectEvent(Region region)
    {
        Region = region;
    }
}