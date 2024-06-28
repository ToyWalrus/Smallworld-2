using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Logic;
using Smallworld.Logic.FSM;
using Smallworld.Models;
using Smallworld.Models.Powers;
using Smallworld.Models.Races;
using Smallworld.Scripts.Events;
using Smallworld.Utils;
using Spectre.Console;

using SWRegion = Smallworld.Models.Region;

namespace ConsoleApp;

internal class Program
{
    private static IServiceProvider _serviceProvider;
    private static IEventAggregator EventAggregator => _serviceProvider.GetRequiredService<IEventAggregator>();
    private static GameFlow _gameFlow;
    private static IGame _game => _gameFlow.Game;
    private static List<string> _turnActions = new();
    private static string _currentStateName;

    public static void Main(string[] args)
    {
        Logger.SetType<AnsiLogger>();

        // http://www.figlet.org/
        AnsiConsole.Write(new FigletText("Smallworld").Centered().Color(Color.Silver));
        var numPlayers = 0;
        do
        {
            numPlayers = AnsiConsole.Ask<int>("How many players? (2-4)");
        } while (numPlayers < 2 || numPlayers > 4);

        AnsiConsole.Clear();

        _serviceProvider = ConfigureServices(numPlayers);
        SetupListeners();

        _gameFlow = new GameFlow();
        _gameFlow.StartGame(_serviceProvider);

        while (!_gameFlow.IsEnded)
        {
            Thread.Sleep(2000);
        }
    }

    private static void SetupListeners()
    {
        EventAggregator.Subscribe<ChangeStateEvent>(OnChangeState);
        EventAggregator.Subscribe<RegionConqueredEvent>(OnRegionConquered);
        EventAggregator.Subscribe<ChangeTurnEvent>(OnChangeTurn);
        EventAggregator.Subscribe<DiceRollResultEvent>(OnRollDiceResult);
    }

    private static void OnChangeTurn(ChangeTurnEvent e)
    {
        _turnActions.Clear();
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"{e.NewPlayer.Name}'s turn").Justify(Justify.Center));
    }

    private static void OnRegionConquered(RegionConqueredEvent e)
    {
        var region = e.Region;
        _turnActions.Add($"[bold {MarkupHelper.GetRegionStringColor(region)}]{region.Name}[/] conquered by [bold {MarkupHelper.GetRaceStringColor(region.OccupiedBy.Race)}]{region.OccupiedBy.Name}[/]");
    }

    private static async void OnChangeState(ChangeStateEvent e)
    {
        _currentStateName = e.NewState.Name;

        if (e.NewState is TurnPlayState)
        {
            PlayerTurnStep();
        }
        // Need to check for old state being TurnStartState because for some reason this event is being
        // triggered twice with the new state being SelectNewRacePowerState (it might have something to do
        // with multiple threads?)
        else if (e.OldState is TurnStartState && e.NewState is SelectNewRacePowerState)
        {
            AnsiConsole.Write(GameRenderer.GetPlayerTable(_game, _gameFlow.CurrentPlayerIndex));
            var selectedPower = await _serviceProvider.GetRequiredService<ISelection<RacePower>>().SelectAsync(_game.AvailableRacePowers);

            var vpUsed = _game.ReplaceRacePower(selectedPower);
            _gameFlow.CurrentPlayer.Player.AddScore(-vpUsed);
        }
    }

    private static void PlayerTurnStep()
    {
        GameRenderer.RenderPlayerTurnStep(_gameFlow.Game, _gameFlow.CurrentPlayer, _turnActions);

        var choices = new List<string> { "Conquer", "End turn" };
        if (_gameFlow.CurrentPlayer.ActiveRacePowers.FirstOrDefault()?.CanEnterDecline() == true)
        {
            choices.Insert(1, "Enter decline");
        }

        var choice = AnsiConsole.Prompt(
             new SelectionPrompt<string>()
                 .Title("What would you like to do?")
                 .PageSize(3)
                 .AddChoices(choices)
             );

        if (choice == "Enter decline")
        {
            EventAggregator.Publish(UIInteractionEvent.EnterDecline);
        }
        else if (choice == "End turn")
        {
            EventAggregator.Publish(UIInteractionEvent.EndTurn);
        }
        else if (choice == "Conquer")
        {
            ConquerPhase();
        }
    }

    private static void OnRollDiceResult(DiceRollResultEvent e)
    {
        _turnActions.Add($"Rolled [bold]{e.Result}[/] on the die");
    }

    private static void ConquerPhase()
    {
        var allRegions = _game.Regions;
        var conquerable = allRegions.Where(_gameFlow.CurrentPlayer.CanConquerRegion).ToList();

        _serviceProvider.GetRequiredService<ISelection<SWRegion>>().SelectAsync(conquerable);
    }

    private static ServiceProvider ConfigureServices(int numPlayers)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IGame, Game>((serviceProvider) => Game.GetPresetGame(serviceProvider, numPlayers));
        services.AddSingleton<IRollDice, DiceRoller>((serviceProvider) => new DiceRoller(serviceProvider, CustomProbabilityDice.Reinforcement));
        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddTransient<IConfirmation, Confirmation>();
        services.AddTransient<ISelection<SWRegion>, RegionSelection>();
        services.AddTransient<ISelection<RacePower>, RacePowerSelection>();
        services.AddTransient<ISelection<Player>, PlayerSelection>();
        services.AddTransient<IModelFactory<Power>, PowerFactory>();
        services.AddTransient<IModelFactory<Race>, RaceFactory>();

        return services.BuildServiceProvider();
    }
}

