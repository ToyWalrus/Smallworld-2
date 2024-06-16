using System;

namespace Smallworld.Logic.FSM;

public class ReinforceState : State
{
    public override string Name => "Reinforce";

    public ReinforceState(StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        throw new NotImplementedException();
    }

    public override void Exit()
    {
        throw new NotImplementedException();
    }
}
