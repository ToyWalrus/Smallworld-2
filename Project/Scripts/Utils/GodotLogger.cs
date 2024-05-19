using Godot;

namespace Smallworld.Utils
{
    public class GodotLogger : Logger
    {
        protected override void Log(LogType type, string message)
        {
            if (type == LogType.Error)
            {
                GD.PushError(message);
            }
            else if (type == LogType.Warning)
            {
                GD.PushWarning(message);
            }
            else
            {
                GD.Print(message);
            }
        }
    }
}