using UnityEngine;

public class UnityLogger : Smallworld.Utils.Logger
{
    protected override void Log(LogType type, string message)
    {
        if (type == LogType.Error)
        {
            Debug.LogError(message);
        }
        else if (type == LogType.Warning)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}
