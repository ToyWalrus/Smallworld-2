using System.Linq;
using Smallworld.Events;

namespace Smallworld.Logic.FSM;

public class TurnPlayState : State
{
    public override string Name => "Turn play";

    public bool CanEnterDecline => CurrentPlayer.ActiveRacePowers.Any(rp => rp.CanEnterDecline());
    public bool IsFirstTurn { get; private set; }

    public TurnPlayState(StateMachine stateMachine, bool isFirstTurn = false) : base(stateMachine)
    {
        IsFirstTurn = isFirstTurn;
    }

    public override void Enter()
    {
        EventAggregator.Subscribe<RegionSelectEvent>(OnRegionSelected);
        EventAggregator.Subscribe<UIInteractionEvent>(OnUIInteraction);

        if (IsFirstTurn)
        {
            StartRacePowerTurns();
        }
    }

    public override void Exit()
    {
        EventAggregator.Unsubscribe<RegionSelectEvent>(OnRegionSelected);
        EventAggregator.Unsubscribe<UIInteractionEvent>(OnUIInteraction);
    }

    private void OnRegionSelected(RegionSelectEvent e)
    {
        ChangeState<ConquerState>(e.Region);
    }

    private void StartRacePowerTurns()
    {
        foreach (var rp in CurrentPlayer.ActiveRacePowers)
        {
            rp.OnTurnStart();
        }
    }

    private void OnUIInteraction(UIInteractionEvent e)
    {
        switch (e.InteractionType)
        {
            case UIInteractionEvent.Types.EndTurn:
                ChangeState<ReinforceState>();
                break;
            case UIInteractionEvent.Types.EnterDecline:
                if (!CanEnterDecline)
                    return;
                CurrentPlayer.EnterDecline();
                ChangeState<TurnEndState>();
                break;
        }
    }
}