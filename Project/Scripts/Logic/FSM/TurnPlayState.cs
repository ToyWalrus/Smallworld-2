using System.Linq;
using System.Threading.Tasks;
using Smallworld.Events;
using Smallworld.Models.Powers;

namespace Smallworld.Logic.FSM;

public class TurnPlayState : State
{
    public override string Name => "Turn play";

    private bool canEnterDecline = true;

    public TurnPlayState(FSM stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        EventAggregator.Subscribe<RegionSelectEvent>(OnRegionSelected);
        EventAggregator.Subscribe<UIInteractionEvent>(OnUIInteraction);
        StartRacePowerTurns();
    }

    public override void Exit()
    {
        EventAggregator.Unsubscribe<RegionSelectEvent>(OnRegionSelected);
        EventAggregator.Unsubscribe<UIInteractionEvent>(OnUIInteraction);
    }

    private async void OnRegionSelected(RegionSelectEvent e)
    {
        var racePowerToUse = CurrentPlayer.ActiveRacePowers.FirstOrDefault(rp => rp.IsValidConquerRegion(e.Region).Item1);
        if (racePowerToUse != null)
        {
            var numTokensToUse = await racePowerToUse.GetFinalRegionConquerCost(e.Region);
            e.Region.Conquer(racePowerToUse, numTokensToUse);
            canEnterDecline = CurrentPlayer.ActiveRacePowers.Any(rp => rp.Power is Stout && !rp.IsInDecline);
        }
    }

    private void StartRacePowerTurns()
    {
        foreach (var rp in CurrentPlayer.ActiveRacePowers)
        {
            rp.OnTurnStart();
        }
    }

    private async void OnUIInteraction(UIInteractionEvent e)
    {
        switch (e.InteractionType)
        {
            case UIInteractionEvent.Types.EndTurn:
                ChangeTurn();
                ChangeState<TurnStartState>();
                break;
            case UIInteractionEvent.Types.EnterDecline:
                if (!canEnterDecline) return;
                CurrentPlayer.EnterDecline();
                await EndRacePowerTurns();
                ChangeTurn();
                ChangeState<TurnStartState>();
                break;

        }
    }

    private async Task EndRacePowerTurns()
    {
        foreach (var rp in CurrentPlayer.ActiveRacePowers)
        {
            await rp.OnTurnEnd();
        }

        CurrentPlayer.TallyVP();
    }
}