using System;
using Microsoft.Extensions.DependencyInjection;
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
        var newState = (T) Activator.CreateInstance(typeof(T), _stateMachine, args);
        _stateMachine.ChangeState(newState);
    }

    protected T GetRequiredService<T>() => _stateMachine.serviceProvider.GetRequiredService<T>();

    protected void ChangeTurn()
    {
        _stateMachine.OnChangeTurn?.Invoke(CurrentPlayer);
    }
}
