using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
                options.Rules.Add(new LoggerFilterRule(
                    "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider",
                    null,
                    LogLevel.Trace,
                    (category, provider, logLevel) => true));
            }
        });
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .Build();

host.Run();
