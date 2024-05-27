using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smallworld.Models;
using Smallworld.Utils;

namespace Smallworld.Logic;

public class GameFlow
{
    public Game Game { get; private set; }
    public Player CurrentPlayer
    {
        get
        {
            if (Game == null)
            {
                Logger.LogError("Game not set, cannot get current player");
                return null;
            }
            return Game.Players[playerTurnIndex];
        }
    }

    private int playerTurnIndex = 0;
    private List<PlayerFlow> playerFlows = new();
    private int round = 0;

    public GameFlow() { }
    public GameFlow(Game game)
    {
        Game = game;
    }

    public void SetGame(Game game)
    {
        Game = game;
    }

    public void SetStartPlayer(Player p)
    {
        playerTurnIndex = Game.Players.IndexOf(p);
    }

    public void SetStartPlayer(int index)
    {
        playerTurnIndex = index % Game.Players.Count;
    }

    public async Task StartGame(IServiceProvider serviceProvider)
    {
        if (Game == null)
        {
            Logger.LogError("Game not set, cannot start game");
            return;
        }

        if (!Game.Players.Any())
        {
            Logger.LogError("No players added to the game, cannot start game");
            return;
        }

        InitPlayerFlows(serviceProvider);

        for (round = 1; round <= Game.NumRounds; round++)
        {
            await StartRound();
        }
    }

    private async Task StartRound()
    {
        do
        {
            await DoPlayerTurn();
        } while (MoveToNextPlayer());
    }

    private async Task DoPlayerTurn()
    {
        var currentPlayerFlow = playerFlows[playerTurnIndex];
        await currentPlayerFlow.DoTurnFlow();
    }

    private bool MoveToNextPlayer()
    {
        playerTurnIndex = (playerTurnIndex + 1) % Game.Players.Count;
        return playerTurnIndex != 0;
    }

    private void InitPlayerFlows(IServiceProvider serviceProvider)
    {
        foreach (var player in Game.Players)
        {
            playerFlows.Add(new PlayerFlow(serviceProvider, player, Game));
        }
    }
}