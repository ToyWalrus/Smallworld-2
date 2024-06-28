using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;
using Spectre.Console;

namespace ConsoleApp;

internal class RacePowerSelection : ISelection<RacePower>
{
    private IServiceProvider _serviceProvider;

    public RacePowerSelection(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<RacePower> SelectAsync(List<RacePower> items)
    {
        var choice = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<RacePower>()
                .Title("Select a RacePower")
                .PageSize(10)
                .AddChoices(items)
                .UseConverter(rp => $"[{MarkupHelper.GetRaceStringColor(rp.Race)}]{rp.Name}[/]")
         ));

        _serviceProvider.GetRequiredService<IEventAggregator>().Publish(new RacePowerSelectEvent(choice));

        return choice;
    }
}
