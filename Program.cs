// Import necessary namespaces
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    // Configure the Functions Web Application
    .ConfigureFunctionsWebApplication()
    // Configure services for dependency injection
    .ConfigureServices(services => {
        // Add Application Insights telemetry for worker service
        services.AddApplicationInsightsTelemetryWorkerService();
        // Configure Application Insights for Azure Functions
        services.ConfigureFunctionsApplicationInsights();
    })
    // Configure logging
    .ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            // Find the default Application Insights logger rule
            LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                // Remove the default rule and add a custom rule to log all levels
                options.Rules.Remove(defaultRule);
                options.Rules.Add(new LoggerFilterRule(
                    "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider",
                    null,
                    LogLevel.Trace,
                    (category, provider, logLevel) => true));
            }
        });
        // Set the minimum log level
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    // Build the host
    .Build();

// Run the host
host.Run();

// LogLevel values:
// Trace (0), Debug (1), Information (2), Warning (3), Error (4), Critical (5), None (6)
