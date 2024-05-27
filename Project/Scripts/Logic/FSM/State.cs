using System;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;

namespace Smallworld.Logic.FSM;

public abstract class State
{
    public abstract string Name { get; }
    public GamePlayer CurrentPlayer => _stateMachine.CurrentPlayer;

    protected IEventAggregator EventAggregator => _stateMachine.EventAggregator;

    private readonly FSM _stateMachine;

    public State(FSM stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Exit();

    protected void ChangeState<T>() where T : State
    {
        var newState = (T)Activator.CreateInstance(typeof(T), _stateMachine);
        _stateMachine.ChangeState(newState);
    }

    protected T GetRequiredService<T>() => _stateMachine.serviceProvider.GetRequiredService<T>();
}
