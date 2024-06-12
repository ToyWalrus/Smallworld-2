using Smallworld.Models;
using Smallworld.Models.Races;
using SWRegion = Smallworld.Models.Region;

namespace ConsoleApp;

internal class MarkupHelper
{

    public static string RegionToMarkupString(SWRegion region)
    {
        return RegionToMarkupString(region, true, true);
    }

    public static string RegionToMarkupString(SWRegion region, bool showOccupiedBy, bool showConquerCost)
    {
        var str = $"[{GetRegionStringColor(region)}]{region.Name}[/]";

        if (showOccupiedBy)
        {
            if (region.OccupiedBy != null)
            {
                str += $" [red](Occupied by {region.OccupiedBy.Name})[/]";
            }
            else if (region.HasToken(Token.LostTribe))
            {
                str += " [yellow](Lost Tribe)[/]";
            }
        }

        if (showConquerCost)
        {
            str += $" [white][[{region.GetBaseConquerCost()}]][/]";
        }

        return str;
    }

    public static string GetRegionStringColor(SWRegion region)
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

    public static string GetRaceStringColor(Race race)
    {
        switch (race.Name.ToLower())
        {
            case "amazons":
                return "green3";
            case "dwarves":
                return "rosybrown";
            case "elves":
                return "lime";
            case "giants":
                return "darkorange3_1";
            case "halflings":
                return "gold1";
            case "humans":
                return "lightskyblue1";
            case "orcs":
                return "darkolivegreen3";
            case "ratmen":
                return "grey53";
            case "skeletons":
                return "lightslategrey";
            case "sorcerers":
                return "purple";
            case "tritons":
                return "deepskyblue3";
            case "trolls":
                return "darkcyan";
            case "wizards":
                return "magenta3_2";
            default:
                return "white";
        }
    }
}
