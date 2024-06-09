using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smallworld.Logic.FSM;

namespace Smallworld.Events;

public class ChangeStateEvent : IEvent
{
    public string Name => "Change state";
    public State NewState { get; private set; }
    public State OldState { get; private set; }

    public ChangeStateEvent(State oldState, State newState)
    {
        OldState = oldState;
        NewState = newState;
    }
}
