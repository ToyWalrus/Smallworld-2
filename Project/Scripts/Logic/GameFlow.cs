using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smallworld.Logic.FSM;
using Smallworld.Models;
using Smallworld.Utils;


namespace Smallworld.Logic;

public class GameFlow
{
    public Game Game { get; private set; }
    private StateMachine stateMachine;
    private List<GamePlayer> players;
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

    public void StartGame(IServiceProvider serviceProvider)
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

        InitGame(serviceProvider);

        stateMachine.ChangeState(new TurnStartState(stateMachine));
    }

    private void InitGame(IServiceProvider serviceProvider)
    {
        foreach (var player in Game.Players)
        {
            players.Add(new GamePlayer(player));
        }

        stateMachine = new StateMachine(serviceProvider);
        stateMachine.OnChangeTurn += ChangePlayerTurn;
        stateMachine.SetCurrentPlayer(players[0]);
    }

    private void ChangePlayerTurn(GamePlayer prevPlayer)
    {
        var oldPlayerIndex = players.IndexOf(prevPlayer);
        var newPlayerIndex = (oldPlayerIndex + 1) % players.Count;

        if (newPlayerIndex == 0)
        {
            round++;
        }

        stateMachine.SetCurrentPlayer(players[newPlayerIndex]);
    }
}