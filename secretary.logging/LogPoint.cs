using Microsoft.Extensions.Logging;
using secretary.configuration;

namespace secretary.logging;

public class LogPoint
{
    private static ILoggerFactory _loggerFactory;

    static LogPoint()
    {
        _loggerFactory = LoggerFactory.Create(builder => Configure(builder));
    }

    private static ILoggingBuilder Configure(ILoggingBuilder builder)
    {
        builder.AddConsole();

        if (Config.Instance.Environment == "Develop")
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        }
        else
        {
            builder.SetMinimumLevel(LogLevel.Information);
        }

        return builder;
    }
    
    public static ILogger<T> GetLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }
}