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
    private static GameFlow gameFlow;
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
        AnsiConsole.Write(new FigletText("Smallworld").Centered().Color(Color.Silver));

        _serviceProvider = ConfigureServices(numPlayers);
        SetupListeners();

        gameFlow = new GameFlow();
        gameFlow.StartGame(_serviceProvider);

        while (!gameFlow.IsEnded)
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
        regionsConquered.Clear();
    }

    private static void OnRegionConquered(RegionConqueredEvent e)
    {
        AnsiConsole.MarkupLine($"[bold]{e.Region.Name}[/] conquered by [bold]{e.Conqueror.Name}[/]");
        ShowInitialTurnUI(false);
    }

    private static void OnChangeState(ChangeStateEvent e)
    {
        AnsiConsole.MarkupLine($"[bold]{e.NewState.Name}[/]");

        if (e.NewState is TurnPlayState)
        {
            ShowInitialTurnUI(true);
        }
    }

    private static void ShowInitialTurnUI(bool canEnterDecline)
    {
        string choice = "";
        do
        {
            GameRenderer.RenderPlayerTurnStep(gameFlow.Game, gameFlow.CurrentPlayer, regionsConquered);

            choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(3)
                    .AddChoices(["Conquer", "Enter decline", "End turn"])
                );

            if (choice == "Enter decline")
            {
                if (!canEnterDecline)
                {
                    choice = "";
                }
                else
                {
                    EventAggregator.Publish(new UIInteractionEvent(UIInteractionEvent.Types.EnterDecline));
                }
            }
            else if (choice == "End turn")
            {
                EventAggregator.Publish(new UIInteractionEvent(UIInteractionEvent.Types.EndTurn));
            }
            else if (choice == "Conquer")
            {
                ConquerPhase();
            }
        } while (choice == "");
    }

    private static async void ConquerPhase()
    {
        var allRegions = gameFlow.Game.Regions;
        var conquerable = allRegions.Where(r => gameFlow.CurrentPlayer.CanConquerRegion(r)).ToList();

        var region = await _serviceProvider.GetRequiredService<ISelection<SWRegion>>().SelectAsync(conquerable);

        if (region != null)
        {
            regionsConquered.Add(region);
        }
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

