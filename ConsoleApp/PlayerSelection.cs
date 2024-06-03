using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;
using Spectre.Console;

namespace ConsoleApp;

internal class PlayerSelection : ISelection<Player>
{
    private IServiceProvider _serviceProvider;

    public PlayerSelection(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Player> SelectAsync(List<Player> items)
    {
        var choice = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<Player>()
                .Title("Select a player")
                .AddChoices(items)
        ));

        _serviceProvider.GetRequiredService<IEventAggregator>().Publish(new PlayerSelectEvent(choice));

        return choice;
    }
}