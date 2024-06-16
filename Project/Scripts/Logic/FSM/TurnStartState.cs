using System.Linq;

namespace Smallworld.Logic.FSM;

public class TurnStartState : State
{
    public override string Name => "Turn start";

    public TurnStartState(StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // If the player did not enter decline last turn and has active
        // racepowers, we can skip this state and go directly to TurnPlayState.
        if (!CurrentPlayer.DidEnterDeclineLastTurn && CurrentPlayer.ActiveRacePowers.Any())
        {
            ChangeState<TurnPlayState>(true);
        }
        else
        {
            ChangeState<SelectNewRacePowerState>();
        }
    }
}