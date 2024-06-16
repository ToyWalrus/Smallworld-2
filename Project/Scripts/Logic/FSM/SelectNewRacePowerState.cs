using Smallworld.Events;

namespace Smallworld.Logic.FSM;

public class SelectNewRacePowerState : State
{
    public override string Name => "Select new race power";

    public SelectNewRacePowerState(StateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        EventAggregator.Subscribe<RacePowerSelectEvent>(OnRacePowerSelected);
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
            ChangeState<TurnPlayState>(true);
        }
    }
}
