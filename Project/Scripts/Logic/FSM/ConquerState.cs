using System.Linq;
using Smallworld.Events;
using Smallworld.Models;
using Smallworld.Utils;

namespace Smallworld.Logic.FSM;

public class ConquerState : State
{
    public override string Name => "Conquer";
    public Region RegionToConquer { get; private set; }

    public ConquerState(StateMachine stateMachine, Region regionToConquer) : base(stateMachine)
    {
        RegionToConquer = regionToConquer;
    }

    public override void Enter()
    {
        ConquerRegion();
    }

    private async void ConquerRegion()
    {
        var racePowerToUse = CurrentPlayer.ActiveRacePowers.FirstOrDefault(rp => rp.IsValidConquerRegion(RegionToConquer).Item1);
        if (racePowerToUse != null)
        {
            var numTokensToUse = await racePowerToUse.GetFinalRegionConquerCost(RegionToConquer);
            if (racePowerToUse.AvailableTokenCount < numTokensToUse)
            {
                Logger.LogMessage($"Not enough tokens to conquer region: {RegionToConquer}");
                ChangeState<TurnPlayState>();
                return;
            }

            RegionToConquer.Conquer(racePowerToUse, numTokensToUse);
            EventAggregator.Publish(new RegionConqueredEvent(RegionToConquer));
        }
    }
}
