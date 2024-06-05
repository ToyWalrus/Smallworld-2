using Smallworld.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

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

    public static Table GetPlayerTable(IGame game)
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
            var isActive = i == game.CurrentPlayerIndex;
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
}