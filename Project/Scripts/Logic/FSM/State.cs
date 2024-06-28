using System;
using Smallworld.Events;

namespace Smallworld.Logic.FSM;

public abstract class State
{
    public abstract string Name { get; }
    public GamePlayer CurrentPlayer => _stateMachine.CurrentPlayer;

    protected IEventAggregator EventAggregator => _stateMachine.EventAggregator;

    private readonly StateMachine _stateMachine;

    public State(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }

    protected void ChangeState<T>(params object[] args) where T : State
    {
        var fullArgs = new object[args.Length + 1];
        fullArgs[0] = _stateMachine;
        args.CopyTo(fullArgs, 1);

        var newState = (T) Activator.CreateInstance(typeof(T), fullArgs);
        _stateMachine.ChangeState(newState);
    }

    protected void ChangeTurn()
    {
        _stateMachine.OnChangeTurn?.Invoke(CurrentPlayer);
    }
}
