using System.Text;
using secretary.configuration;
using Serilog;
using Serilog.Core;

namespace secretary.logging;

public class LogPoint
{
    private static LoggerConfiguration _configuration;

    static LogPoint()
    {
        _configuration = Configure();

        Log.Logger = _configuration.CreateLogger();
    }
    private static LoggerConfiguration Configure()
    {
        var template = "{Timestamp:HH:mm:ss} [{Level}] — {SourceContext} — {Message}{NewLine}{Exception}";
        
        var configuration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: template)
            .WriteTo.File("logs/logs.txt", rollingInterval: RollingInterval.Month, outputTemplate: template);
        
        if (Config.Instance.Environment == "Develop")
        {
            configuration.MinimumLevel.Debug();
        }
        else
        {
            configuration.MinimumLevel.Information();
        }
        
        return configuration;
    }
    
    public static ILogger GetLogger<T>()
    {
        return Log.Logger.ForContext<T>();
    }
}