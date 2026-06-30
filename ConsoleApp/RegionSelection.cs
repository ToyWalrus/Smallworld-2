using Microsoft.Extensions.DependencyInjection;
using Smallworld.Events;
using Smallworld.IO;
using Smallworld.Models;
using Spectre.Console;

using SWRegion = Smallworld.Models.Region;

namespace ConsoleApp;

internal class RegionSelection : ISelection<SWRegion>
{
    IServiceProvider _serviceProvider;

    public RegionSelection(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<SWRegion> SelectAsync(List<SWRegion> items)
    {
        var choice = await Task.Run(() => AnsiConsole.Prompt(new SelectionPrompt<SWRegion>()
            .Title("Select a region")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more regions)[/]")
            .AddChoices(items)
            .UseConverter(MarkupHelper.RegionToMarkupString)
        ));

        _serviceProvider.GetRequiredService<IEventAggregator>().Publish(new RegionSelectEvent(choice));

        return choice;
    }
}

