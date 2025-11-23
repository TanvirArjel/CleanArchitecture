using Microsoft.Extensions.Logging;

namespace CleanHr.Application.Extensions;

public static class LoggerExtensions
{
#pragma warning disable CA2254 // Template should be a static expression
    public static void LogWithLevel<T>(
        this ILogger<T> logger,
        LogLevel logLevel,
        string message)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, message);
        }
    }

    public static void LogWithLevel<T>(
        this ILogger<T> logger,
        LogLevel logLevel,
        string message,
        Dictionary<string, object> fields)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, message, fields);
        }
    }

    public static void LogException<T>(
        this ILogger<T> logger,
        Exception exception,
        string message,
        Dictionary<string, object> fields)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Error))
        {
            logger.Log(LogLevel.Error, exception, message, fields);
        }
    }
#pragma warning restore CA2254
}
