using System;
using Smallworld.Utils;

namespace Smallworld.Logic.FSM;

public class ReinforceState : State
{
    public override string Name => "Reinforce";

    public ReinforceState(StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Logger.LogWarning("ReinforceState not implemented yet");
        ChangeState<TurnEndState>();
    }
}
