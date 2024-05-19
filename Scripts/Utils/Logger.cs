namespace Smallworld.Utils
{
    public abstract class Logger
    {
        public static Logger _logger = new SystemLogger();

        public static void SetType<T>() where T : Logger, new()
        {
            _logger = new T();
        }

        public static void LogMessage(string message)
        {
            _logger.Log(LogType.Message, message);
        }

        public static void LogError(string message)
        {
            _logger.Log(LogType.Error, message);
        }

        public static void LogWarning(string message)
        {
            _logger.Log(LogType.Warning, message);
        }

        protected abstract void Log(LogType type, string message);

        protected enum LogType
        {
            Message,
            Warning,
            Error
        }
    }
}