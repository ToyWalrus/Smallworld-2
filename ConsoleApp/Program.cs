using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;
using Smallworld.Utils;

namespace ConsoleApp;

internal class Program
{
    public static void Main(string[] args)
    {
        Logger.SetType<AnsiLogger>();

        var serviceProvider = ConfigureServices();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IGame, Game>((serviceProvider) => Game.GetPresetGame(serviceProvider));
        services.AddSingleton<IRollDice, DiceRoller>((serviceProvider) => new DiceRoller(serviceProvider, CustomProbabilityDice.Reinforcement));
        services.AddTransient<IConfirmation, Confirmation>();
        services.AddTransient<ISelection<Region>, RegionSelection>();
        services.AddTransient<ISelection<RacePower>, RacePowerSelection>();
        services.AddTransient<ISelection<Player>, PlayerSelection>();
        services.AddTransient<IEventAggregator, EventAggregator>();

        return services.BuildServiceProvider();
    }
}

