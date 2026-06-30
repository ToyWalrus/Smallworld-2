namespace Smallworld.Utils
{
    public class SystemLogger : Logger
    {
        protected override void Log(LogType type, string message)
        {
            if (type == LogType.Error)
            {
                System.Console.Error.WriteLine(message);
            }
            else if (type == LogType.Warning)
            {
                System.Console.WriteLine($"[WARNING]: {message}");
            }
            else
            {
                System.Console.WriteLine(message);
            }
        }
    }
}