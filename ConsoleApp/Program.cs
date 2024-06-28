using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Logic;
using Smallworld.Logic.FSM;
using Smallworld.Models;
using Smallworld.Models.Powers;
using Smallworld.Models.Races;
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
    private static List<SWRegion> regionsConquered = new();

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
    }

    private static void OnChangeTurn(ChangeTurnEvent e)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"{e.NewPlayer.Name}'s turn").Justify(Justify.Center));
        regionsConquered.Clear();
    }

    private static void OnRegionConquered(RegionConqueredEvent e)
    {
        regionsConquered.Add(e.Region);
        PlayerTurnStep(false);
    }

    private static async void OnChangeState(ChangeStateEvent e)
    {
        if (e.NewState is TurnPlayState)
        {
            PlayerTurnStep(true);
        }
        else if (e.NewState is SelectNewRacePowerState)
        {
            AnsiConsole.Write(GameRenderer.GetPlayerTable(_game, _gameFlow.CurrentPlayerIndex));
            var selectedPower = await _serviceProvider.GetRequiredService<ISelection<RacePower>>().SelectAsync(_game.AvailableRacePowers);
            var vpUsed = _game.ReplaceRacePower(selectedPower);
            _gameFlow.CurrentPlayer.Player.AddScore(-vpUsed);
        }
    }

    private static void PlayerTurnStep(bool canEnterDecline)
    {

        GameRenderer.RenderPlayerTurnStep(_gameFlow.Game, _gameFlow.CurrentPlayer, regionsConquered);

        var choices = new List<string> { "Conquer", "End turn" };
        if (canEnterDecline)
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
        else
        {
            ConquerPhase();
        }
    }

    private static void ConquerPhase()
    {
        var allRegions = _gameFlow.Game.Regions;
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

