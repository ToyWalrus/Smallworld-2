using Smallworld.Utils;
using Spectre.Console;

namespace ConsoleApp;

internal class AnsiLogger : Logger
{
    protected override void Log(LogType type, string message)
    {
        if (type == LogType.Error)
        {
            AnsiConsole.MarkupLine($"[red][[ERROR]][/]: {message}");
        }
        else if (type == LogType.Warning)
        {
            AnsiConsole.MarkupLine($"[yellow][[WARNING]][/]: {message}");
        }
        else
        {
            AnsiConsole.MarkupLine($"[dodgerblue3][[INFO]][/]: {message}");
        }
    }
}