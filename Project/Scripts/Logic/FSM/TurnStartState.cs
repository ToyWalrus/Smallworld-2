using System.Linq;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;

namespace Smallworld.Logic.FSM;

public class TurnStartState : State
{
    public override string Name => "Turn start";

    public TurnStartState(FSM stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // If the player did not enter decline last turn and has active
        // racepowers, we can skip this state and go directly to TurnPlayState.
        if (!CurrentPlayer.DidEnterDeclineLastTurn && CurrentPlayer.ActiveRacePowers.Any())
        {
            ChangeState<TurnPlayState>();
        }
        else
        {
            EventAggregator.Subscribe<RacePowerSelectEvent>(OnRacePowerSelected);
            SetReadyToSelectRacePower();
        }
    }

    private void SetReadyToSelectRacePower()
    {
        GetRequiredService<ISelection<RacePower>>().SelectAsync(
            GetRequiredService<IGame>().AvailableRacePowers
        );
    }

    public override void Exit()
    {
        EventAggregator.Unsubscribe<RacePowerSelectEvent>(OnRacePowerSelected);
    }

    private void OnRacePowerSelected(RacePowerSelectEvent e)
    {
        if (e.OwningPlayer == null)
        {
            CurrentPlayer.AddRacePower(e.RacePower);
            CurrentPlayer.DidEnterDeclineLastTurn = false;
            ChangeState<TurnPlayState>();
        }
    }
}