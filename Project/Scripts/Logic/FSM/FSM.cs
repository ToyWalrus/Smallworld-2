using System;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;

namespace Smallworld.Logic.FSM;

public class FSM
{
    public delegate void ChangeTurnHandler(GamePlayer prevPlayer);
    public ChangeTurnHandler OnChangeTurn;

    public State CurrentState { get; private set; }
    public GamePlayer CurrentPlayer { get; private set; }
    public IEventAggregator EventAggregator => serviceProvider.GetRequiredService<IEventAggregator>();
    public readonly IServiceProvider serviceProvider;

    public FSM(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void SetCurrentPlayer(GamePlayer player)
    {
        CurrentPlayer = player;
    }

    public void ChangeState(State newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}