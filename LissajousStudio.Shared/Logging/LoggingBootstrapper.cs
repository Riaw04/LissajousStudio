using Serilog;

namespace LissajousStudio.Shared.Logging;

/// <summary>Configures structured file logging for the desktop application.</summary>
public static class LoggingBootstrapper
{
    public static void Configure() => Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File("logs/lissajousstudio-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
}
