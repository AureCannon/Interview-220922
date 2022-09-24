using Serilog;
using Serilog.Events;

namespace Ct.Interview.Web.Api.Installers
{
    public sealed class LoggingInstaller : IInstaller
    {
        public void Configure(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, logConfig) => CreateSerilogConfiguration(context, logConfig));
        }

        private static LoggerConfiguration CreateSerilogConfiguration(HostBuilderContext context, LoggerConfiguration configuration)
        {
            var result = configuration.ReadFrom.Configuration(context.Configuration);

            var minimumLevelMS = Enum.Parse<LogLevel>(context.Configuration.GetSection("Logging").GetSection("LogLevel")["Default"]);
            var serilogMinimumLevel = Convert(minimumLevelMS);

            _ = result.MinimumLevel.ControlledBy(new Serilog.Core.LoggingLevelSwitch(serilogMinimumLevel));

            foreach (var section in context.Configuration.GetSection("Logging").GetSection("LogLevel").GetChildren())
            {
                var @namespace = section.Key;
                if (@namespace == "Default")
                {
                    continue;
                }

                var value = Convert(Enum.Parse<LogLevel>(section.Value));
                _ = result.MinimumLevel.Override(@namespace, value);
            }

            return result;
        }

        private static LogEventLevel Convert(LogLevel minimumLevelMS)
        {
            switch (minimumLevelMS)
            {
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.Debug:
                    return LogEventLevel.Verbose;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.None:
                    return LogEventLevel.Debug;
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                default:
                    return LogEventLevel.Information;
            }
        }
    }
}
