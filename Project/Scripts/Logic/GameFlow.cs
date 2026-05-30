using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.Hooks;
using Smallworld.IO;
using Smallworld.Models;
using Smallworld.Utils;

namespace Smallworld.Logic;

public class GameFlow
{
    public IGame Game { get; private set; }
    public int CurrentRound = 0;

    public int ActivePlayerIndex { get; private set; }
    public Player ActivePlayer => Game.Players[ActivePlayerIndex];


    private TaskCompletionSource<bool> enterDecline = new();
    private TaskCompletionSource<bool> doneConquering = new();
    private readonly int[] vpsOnAvailableRacePowers;
    private readonly IServiceProvider serviceProvider;
    private IHooks Hooks => HooksService.Instance;

    public GameFlow(IServiceProvider serviceProvider, IGame game)
    {
        Game = game;
        ActivePlayerIndex = 0;
        vpsOnAvailableRacePowers = Enumerable.Repeat(0, Game.AvailableRacePowers.Count).ToArray();
        this.serviceProvider = serviceProvider;
    }

    #region Public
    public int GetVPOnRacePowerIndex(int idx)
    {
        if (idx >= vpsOnAvailableRacePowers.Count() || idx < 0)
        {
            return 0;
        }
        return vpsOnAvailableRacePowers[idx];
    }

    public void EnterDeclineButtonPressed()
    {
        enterDecline.TrySetResult(true);
    }

    public void DoneConqueringButtonPressed()
    {
        doneConquering.TrySetResult(true);
    }

    public async Task RunGame()
    {
        // Each player begins with 7 VP
        foreach (var player in Game.Players)
        {
            player.AddScore(7);
        }

        while (CurrentRound < Game.NumRounds)
        {
            await Hooks.Run(new RoundStartHook { RoundNumber = CurrentRound });

            while (ActivePlayerIndex < Game.Players.Count)
            {
                await Hooks.Run(new TurnStartHook { Player = ActivePlayer });

                var rp = await RacePowerSelectionPhase();
                await ConquerPhase(rp);
                await ScorePhase();

                await Hooks.Run(new TurnEndHook { Player = ActivePlayer });

                ActivePlayerIndex++;
            }

            await Hooks.Run(new RoundEndHook { RoundNumber = CurrentRound });

            CurrentRound++;
            ActivePlayerIndex = 0;
        }

        DetermineVictor();
    }
    #endregion

    #region Phases
    private async Task<RacePower> RacePowerSelectionPhase()
    {
        if (ActivePlayer.HasActiveRace) return ActivePlayer.ActiveRacePower;

        await Hooks.Run(new BeforeRacePowerSelectionHook { Player = ActivePlayer });

        var (rp, passedCount, existingVP) = await SelectNewRacePowerFromAvailable();
        ActivePlayer.AddScore(existingVP - passedCount);
        ActivePlayer.AddRacePower(rp);
        Game.ReplaceRacePower(rp);

        await Hooks.Run(new AfterRacePowerSelectionHook { Player = ActivePlayer, Selected = rp });

        return rp;

    }

    private async Task ConquerPhase(RacePower rp)
    {
        enterDecline = new();
        doneConquering = new();

        await Hooks.Run(new ConquerPhaseStartHook { Player = ActivePlayer });

        bool didEnterDecline = false;

        rp.OnTurnStart();

        while (true)
        {
            /*
              How cancellation token works:

              GameFlow (CALLER)          SelectAsync impl (CALLEE)
                   |                              |
                   | creates CTS                  |
                   | passes cts.Token ----------> | receives token (read-only)
                   |                              | waits for user input...
                   |                              | checks: was I canceled?
                   | cts.Cancel() ------------>   | yes → abort, dismiss UI
            */
            using var cts = new CancellationTokenSource();

            var regionSelection = SelectRegionForConquering(rp, cts.Token);
            var winner = await Task.WhenAny(regionSelection, enterDecline.Task, doneConquering.Task);

            if (winner == enterDecline.Task && rp.CanEnterDecline())
            {
                cts.Cancel();
                didEnterDecline = true;
                break;
            }

            if (winner == doneConquering.Task)
            {
                cts.Cancel();
                break;
            }

            var region = await regionSelection;
            var conquerCost = await rp.GetFinalRegionConquerCost(region);

            await Hooks.Run(new BeforeConquerRegionHook { ConquerCount = conquerCost, RacePower = rp, Region = region });
            rp.ConquerRegion(region, conquerCost);
            await Hooks.Run(new AfterConquerRegionHook { FinalConquerCount = conquerCost, RacePower = rp, Region = region });
        }

        if (didEnterDecline)
        {
            ActivePlayer.ClearDeclineRacePowers();
            rp.EnterDecline();
        }
        else
        {
            await RedeployPhase(rp);
        }

        await rp.OnTurnEnd();

        if (rp.IsInDecline)
        {
            await Hooks.Run(new RacePowerEnterDeclineHook { RacePower = rp });
        }
    }

    private async Task RedeployPhase(RacePower rp)
    {
        while (rp.AvailableTokenCount > 0)
        {
            var region = await SelectOwnedRegionForRedeployment(rp);
            region.Reinforce(1);
            rp.SpendToken(1);
        }
    }

    private async Task ScorePhase()
    {

        await Hooks.Run(new BeforeScorePhaseHook { Player = ActivePlayer });

        var vp = ActivePlayer.TallyVP();
        ActivePlayer.AddScore(vp);

        await Hooks.Run(new AfterScorePhaseHook { Player = ActivePlayer, VPScored = vp });
    }
    #endregion

    #region Helpers
    private async Task<(RacePower, int, int)> SelectNewRacePowerFromAvailable()
    {
        var availableVP = ActivePlayer.Score;
        var rpSelector = serviceProvider.GetRequiredService<ISelection<RacePower>>();

        var availableRPs = Game.AvailableRacePowers;
        var selectableRPs = availableRPs.GetRange(0, Math.Min(availableVP, availableRPs.Count));
        var selection = await rpSelector.SelectAsync(selectableRPs, (_) => "Not enough VPs");
        var indexOfSelection = selectableRPs.IndexOf(selection);

        for (int i = 0; i < indexOfSelection; ++i)
        {
            vpsOnAvailableRacePowers[i]++;
        }

        var vpGained = vpsOnAvailableRacePowers[indexOfSelection];
        vpsOnAvailableRacePowers[indexOfSelection] = 0;

        return (selection, indexOfSelection, vpGained);
    }

    private async Task<Region> SelectOwnedRegionForRedeployment(RacePower rp)
    {
        var regionSelector = serviceProvider.GetRequiredService<ISelection<Region>>();
        return await regionSelector.SelectAsync(rp.GetOwnedRegions(), (_) => "Not owned region");
    }

    private async Task<Region> SelectRegionForConquering(RacePower rp, CancellationToken token)
    {
        var regionSelector = serviceProvider.GetRequiredService<ISelection<Region>>();
        var conquerable = Game.Regions.Where(region => region.IsValidConquerTarget(rp).Item1).ToList();
        return await regionSelector.SelectAsync(conquerable, (region) => region.IsValidConquerTarget(rp).Item2, token);
    }


    private void DetermineVictor() { }
    #endregion
}