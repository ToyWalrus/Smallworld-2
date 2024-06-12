using Smallworld.Logic;
using Smallworld.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

using SWRegion = Smallworld.Models.Region;

namespace ConsoleApp;

internal class GameRenderer
{
    private static Color BorderColor = Color.LightSkyBlue3;

    public static IRenderable GetConsoleLayout(IRenderable playerTable, IRenderable gameRenderable, IRenderable inputRenderable)
    {
        var playerInfo = new Layout("PlayerInfo").Update(playerTable);
        var gameInfo = new Layout("GameInfo").Update(new Panel(gameRenderable).BorderColor(BorderColor).Expand());
        var inputArea = new Layout("InputArea").Update(new Panel(inputRenderable).BorderColor(BorderColor).Expand());

        return new Layout("Root").SplitColumns(
            gameInfo.Ratio(5),
            new Layout().SplitRows(playerInfo, inputArea).Ratio(3)
        );
    }

    public static Table GetPlayerTable(IGame game, int currentPlayerIndex)
    {
        var table = new Table()
            .BorderColor(BorderColor)
            .AddColumn(new TableColumn("[bold]Turn[/]").Alignment(Justify.Right))
            .AddColumn("[bold]Player[/]")
            .AddColumn("[bold]Active race[/]")
            .AddColumn("[bold]Score[/]");

        for (int i = 0; i < game.Players.Count; i++)
        {
            var player = game.Players[i];
            var isActive = i == currentPlayerIndex;
            table.AddRow(
                isActive ? "[red bold]X[/]" : "",
                player.Name,
                player.RacePowers.FirstOrDefault(r => !r.IsInDecline)?.Name ?? "-",
                player.Score.ToString()
            );
        }

        // Active race column
        table.Columns[2].Width(40);

        return table;
    }

    public static Rows GetAvailableRacePowers(IGame game)
    {
        var rows = new List<IRenderable>();

        for (int i = 0; i < game.AvailableRacePowers.Count; i++)
        {
            var rp = game.AvailableRacePowers[i];
            rows.Add(new Markup($"[[{i}vp]] [italic]{rp.Name}[/]"));
        }

        return new Rows(rows);
    }

    private static Rows GetRacePowerWithOwnedRegions(RacePower rp, int padding = 2)
    {
        var rowPadding = new Padding(padding, 0, 0, 0);

        var regions = rp.GetOwnedRegions();
        var rows = new List<IRenderable>
        {
            new Markup($"[{MarkupHelper.GetRaceStringColor(rp.Race)}]{rp.Name}[/] ({rp.AvailableTokenCount})")
        };

        foreach (var region in regions)
        {
            rows.Add(
                new Padder(
                    new Markup(MarkupHelper.RegionToMarkupString(
                        region,
                        showOccupiedBy: false,
                        showConquerCost: false,
                        showNumRaceTokens: true
                    )),
                    rowPadding
                )
            );
        }

        return new Rows(rows);
    }

    public static Panel GetPlayerInfo(GamePlayer currentPlayer, bool withBorder = true)
    {
        var padding = 2;
        var activeRaces = currentPlayer.ActiveRacePowers;
        var declineRaces = currentPlayer.Player.RacePowers.Where(rp => rp.IsInDecline);

        var panelRows = new List<IRenderable> { new Markup($"[bold]Active race(s)[/]") };
        var rowPadding = new Padding(padding, 0, 0, 0);

        foreach (var rp in activeRaces)
        {
            panelRows.Add(new Padder(GetRacePowerWithOwnedRegions(rp, padding)).Padding(rowPadding));
        }

        panelRows.Add(new Text(""));
        panelRows.Add(new Markup($"[bold]Race(s) in decline[/]"));

        foreach (var rp in declineRaces)
        {
            panelRows.Add(new Padder(GetRacePowerWithOwnedRegions(rp, padding)).Padding(rowPadding));
        }

        var playerInfoPanel = new Panel(new Rows(panelRows))
            .Header(new PanelHeader(currentPlayer.Name))
            .HeaderAlignment(Justify.Center)
            .Padding(padding, padding);

        if (withBorder)
        {
            playerInfoPanel.RoundedBorder().BorderColor(BorderColor);
        }
        else
        {
            playerInfoPanel.NoBorder();
        }

        return playerInfoPanel;
    }

    public static void RenderPlayerTurnStep(IGame game, GamePlayer currentPlayer, List<SWRegion> conqueredRegions)
    {

        // Clear console
        AnsiConsole.Clear();

        // Render the current game status
        AnsiConsole.Write(
            new Columns(
                GetPlayerTable(game, game.Players.IndexOf(currentPlayer.Player)),
                GetPlayerInfo(currentPlayer).Expand()
            )
        );

        // Start the lines indicating player's actions
        AnsiConsole.Write(new Padder(new Rule($"{currentPlayer.Name}'s turn"), new(2, 1)));

        foreach (var region in conqueredRegions)
        {
            AnsiConsole.MarkupLine($"[bold {
                    MarkupHelper.GetRegionStringColor(region)
                }]{region.Name}[/] conquered by [bold {
                    MarkupHelper.GetRaceStringColor(region.OccupiedBy.Race)
                }]{region.OccupiedBy.Name}[/]");
        }
    }
}