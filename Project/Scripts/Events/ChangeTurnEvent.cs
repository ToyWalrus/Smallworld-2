using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smallworld.Logic;
using Smallworld.Models;

namespace Smallworld.Events;

public class ChangeTurnEvent : IEvent
{
    public string Name => "Change turn";
    public GamePlayer NewPlayer { get; private set; }

    public ChangeTurnEvent(GamePlayer newPlayer)
    {
        NewPlayer = newPlayer;
    }
}
