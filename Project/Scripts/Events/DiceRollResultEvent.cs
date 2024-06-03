using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smallworld.Events;

namespace Smallworld.Scripts.Events;

public class DiceRollResultEvent : IEvent
{
    public string Name => "DiceRollResultEvent";
    public int Result { get; }

    public DiceRollResultEvent(int result)
    {
        Result = result;
    }
}
