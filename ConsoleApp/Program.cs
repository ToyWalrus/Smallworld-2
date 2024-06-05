using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;
using Smallworld.Models.Powers;
using Smallworld.Models.Races;
using Smallworld.Utils;
using Spectre.Console;

using SWRegion = Smallworld.Models.Region;

namespace ConsoleApp;

internal class Program
{
    public static void Main(string[] args)
    {
        Logger.SetType<AnsiLogger>();

        var serviceProvider = ConfigureServices();
        var game = serviceProvider.GetRequiredService<IGame>();

        // http://www.figlet.org/
        AnsiConsole.Write(new FigletText("Smallworld").Centered().Color(Color.Silver));

        // if (!AnsiConsole.Confirm("Start game?"))
        // {
        //     return;
        // }

        var playerTable = GameRenderer.GetPlayerTable(game);
        var gamePanel = new Panel(GameRenderer.GetAvailableRacePowers(game)).NoBorder();
        var inputPanel = new Panel(Align.Center(new Markup("[bold]Input panel[/]"))).NoBorder();

        AnsiConsole.Write(GameRenderer.GetConsoleLayout(playerTable, gamePanel, inputPanel));
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IGame, Game>((serviceProvider) => Game.GetPresetGame(serviceProvider));
        services.AddSingleton<IRollDice, DiceRoller>((serviceProvider) => new DiceRoller(serviceProvider, CustomProbabilityDice.Reinforcement));
        services.AddTransient<IConfirmation, Confirmation>();
        services.AddTransient<ISelection<SWRegion>, RegionSelection>();
        services.AddTransient<ISelection<RacePower>, RacePowerSelection>();
        services.AddTransient<ISelection<Player>, PlayerSelection>();
        services.AddTransient<IEventAggregator, EventAggregator>();
        services.AddTransient<IModelFactory<Power>, PowerFactory>();
        services.AddTransient<IModelFactory<Race>, RaceFactory>();

        return services.BuildServiceProvider();
    }
}

