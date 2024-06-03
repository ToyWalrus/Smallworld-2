using System;
using System.Threading.Tasks;
using Smallworld.Utils;
using Spectre.Console;

namespace ConsoleApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        Logger.SetType<AnsiLogger>();

        var inputTask = GetUserInputAsync();

        //while (!inputTask.IsCompleted)
        //{
        //    AnsiConsole.MarkupLine("[blue]Doing some other work...[/]");
        //    await Task.Delay(1000); // Simulate other work
        //}

        string userInput = await inputTask;

        AnsiConsole.MarkupLine($"[green]User Input: {userInput}[/]");
    }

    static Task<string> GetUserInputAsync()
    {
        return Task.Run(() => AnsiConsole.Ask<string>("What's your name?"));
    }
}

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