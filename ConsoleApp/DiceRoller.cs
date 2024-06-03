using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;
using Smallworld.Scripts.Events;
using Spectre.Console;

namespace ConsoleApp;

internal class DiceRoller : IRollDice
{
    private IServiceProvider _serviceProvider;
    private IDice _dice;

    public DiceRoller(IServiceProvider serviceProvider, IDice dice)
    {
        _serviceProvider = serviceProvider;
        _dice = dice;
    }

    public async Task<int> RollDiceAsync()
    {
        int result = 0;

        await AnsiConsole.Status().StartAsync("Rolling dice...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);

                await Task.Delay(500);
                result = _dice.Roll();

                AnsiConsole.MarkupLine($"[bold]Rolled {result}[/]");
            });

        _serviceProvider.GetRequiredService<IEventAggregator>().Publish(new DiceRollResultEvent(result));

        return result;
    }
}
