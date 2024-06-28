using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Spectre.Console;

namespace ConsoleApp;

internal class Confirmation : IConfirmation
{
    private IServiceProvider _serviceProvider;

    public Confirmation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmLabel = "Yes", string cancelLabel = "No")
    {
        var choice = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold]{title}[/] | {message}")
                .AddChoices(new[] { confirmLabel, cancelLabel })
        ));

        var didConfirm = choice == confirmLabel;

        _serviceProvider.GetRequiredService<IEventAggregator>().Publish(didConfirm ? UIInteractionEvent.Confirm : UIInteractionEvent.Cancel);

        return didConfirm;
    }
}

