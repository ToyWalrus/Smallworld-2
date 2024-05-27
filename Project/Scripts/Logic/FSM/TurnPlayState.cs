namespace Smallworld.Logic.FSM;

public class TurnPlayState : State
{
    public override string Name => "Turn play";

    public TurnPlayState(FSM stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StartRacePowerTurns();
    }

    public override void Exit() { }

    private void StartRacePowerTurns()
    {
        foreach (var rp in CurrentPlayer.ActiveRacePowers)
        {
            rp.OnTurnStart();
        }
    }
}