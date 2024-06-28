﻿using Smallworld.Utils;

namespace Smallworld.Logic.FSM;

public class TurnEndState : State
{
    public override string Name => "Turn end";

    public TurnEndState(StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        EndRacePowerTurns();
    }

    private async void EndRacePowerTurns()
    {
        Logger.LogMessage("End turn");
        foreach (var rp in CurrentPlayer.ActiveRacePowers)
        {
            await rp.OnTurnEnd();
        }

        CurrentPlayer.TallyVP();

        ChangeTurn();
        ChangeState<TurnStartState>();
    }
}
