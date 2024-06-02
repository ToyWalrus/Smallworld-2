using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.IO;
using Smallworld.Models;
using Smallworld.Models.Powers;

namespace Smallworld.Logic;

public class PlayerFlow
{
    public Player Player { get; private set; }

    private bool wentIntoDeclineLastTurn = false;
    private readonly IServiceProvider serviceProvider;
    private readonly Game game;

    public PlayerFlow(IServiceProvider serviceProvider, Player player, Game game)
    {
        if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
        if (Player == null) throw new ArgumentNullException(nameof(player));
        if (game == null) throw new ArgumentNullException(nameof(game));

        Player = player;
        this.serviceProvider = serviceProvider;
        this.game = game;
    }

    public async Task DoTurnFlow()
    {
        if (wentIntoDeclineLastTurn || Player.RacePowers.Count == 0)
        {
            var chosenPower = await ChooseRacePower();
            Player.AddRacePower(chosenPower);
            wentIntoDeclineLastTurn = false;
        }

        StartRacePowerTurns();

        var (selectedRegion, enterDecline) = await RegionSelectionOrDecline();

        if (enterDecline == true)
        {
            EnterDeclineSelected();
        }
        else
        {
            await ConquerRegions(selectedRegion);
            await EndRacePowerTurns();
        }

        ScorePoints();
    }

    private async Task<RacePower> ChooseRacePower()
    {
        var selector = serviceProvider.GetRequiredService<ISelection<RacePower>>();
        var selected = await selector.SelectAsync(game.AvailableRacePowers);
        return selected;
    }

    private async Task<(Region, bool?)> RegionSelectionOrDecline()
    {
        var regionSelector = serviceProvider.GetRequiredService<ISelection<Region>>();
        var declineSelector = serviceProvider.GetRequiredService<ISelection<bool>>();
        var confirmation = serviceProvider.GetRequiredService<IConfirmation>();

        Region selectedRegion = null;
        bool? declineSelected = null;

        do
        {
            var regionSelection = regionSelector.SelectAsync(game.Regions);
            var declineSelection = declineSelector.SelectAsync(new List<bool> { true, false });

            await Task.WhenAny(regionSelection, declineSelection);

            if (regionSelection.Status == TaskStatus.RanToCompletion)
            {
                selectedRegion = regionSelection.Result;
            }
            else if (declineSelection.Status == TaskStatus.RanToCompletion && declineSelection.Result)
            {
                var confirmed = await confirmation.ConfirmAsync("Are you sure you want to go into decline?");
                if (confirmed)
                {
                    declineSelected = true;
                }
            }
        } while (selectedRegion == null && declineSelected == null);

        return (selectedRegion, declineSelected);
    }

    private Task ConquerRegions(Region regionToConquer)
    {
        // TODO: conquer logic; make recursive

        return Task.CompletedTask;
    }

    private void EnterDeclineSelected()
    {
        wentIntoDeclineLastTurn = true;

        // Spirit power does not follow only 1 race in decline rule
        foreach (var rp in Player.RacePowers.Where(rp => rp.IsInDecline && rp.Power is not Spirit))
        {
            rp.AbandonAllRegions();
            Player.RemoveRacePower(rp);
        }

        foreach (var rp in ActiveRacePowers)
        {
            rp.EnterDecline();
        }
    }

    private void ScorePoints()
    {
    }

    private IEnumerable<RacePower> ActiveRacePowers => Player.RacePowers.Where(rp => !rp.IsInDecline);
    private void StartRacePowerTurns()
    {
        foreach (var rp in ActiveRacePowers)
        {
            rp.OnTurnStart();
        }
    }
    private async Task EndRacePowerTurns()
    {
        foreach (var rp in ActiveRacePowers)
        {
            await rp.OnTurnEnd();
        }
    }
}