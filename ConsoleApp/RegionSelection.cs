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
            .UseConverter(RegionToString)
        ));

        _serviceProvider.GetRequiredService<IEventAggregator>().Publish(new RegionSelectEvent(choice));

        return choice;
    }

    private string RegionToString(SWRegion region)
    {
        var str = $"[{GetRegionStringColor(region)}]{region.Name}[/]";

        if (region.OccupiedBy != null)
        {
            str += $" [red](Occupied by {region.OccupiedBy.Name})[/]";
        } else if (region.HasToken(Token.LostTribe))
        {
            str += " [yellow](Lost Tribe)[/]";
        }

        str += $" [white][[{region.GetBaseConquerCost()}]][/]";

        return str;
    }

    private string GetRegionStringColor(SWRegion region)
    {
        switch (region.Type)
        {
            case RegionType.Sea:
                return "blue";
            case RegionType.Hill:
                return "olive";
            case RegionType.Lake:
                return "aqua";
            case RegionType.Mountain:
                return "silver";
            case RegionType.Forest:
                return "green";
            case RegionType.Farmland:
                return "chartreuse4";
            case RegionType.Swamp:
                return "springgreen4";
            default:
                return "white";
        }
    }
}

