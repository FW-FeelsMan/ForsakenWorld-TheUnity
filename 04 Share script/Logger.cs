using UnityEngine;

public enum LogLevel
{
    None,
    Error,
    Warning,
    Info,
    Debug
}

public static class Logger
{
    public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Debug;

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        if (level <= CurrentLogLevel)
        {
            switch (level)
            {
                case LogLevel.Error:
                    Debug.LogError(message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Info:
                case LogLevel.Debug:
                    Debug.Log(message);
                    break;
            }
        }
    }
}