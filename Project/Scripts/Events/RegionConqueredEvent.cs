using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smallworld.Models;

namespace Smallworld.Events;

public class RegionConqueredEvent:IEvent
{
    public string Name => "Region conquered";
    public Region Region { get; private set; }
    public Player Conqueror { get; private set; }

    public RegionConqueredEvent(Region region, Player conqueror)
    {
        Region = region;
        Conqueror = conqueror;
    }
}
