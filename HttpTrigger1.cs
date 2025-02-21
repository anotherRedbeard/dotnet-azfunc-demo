using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Company.Function
{
    public class HttpTrigger1
    {
        private readonly ILogger<HttpTrigger1> _logger;

        public HttpTrigger1(ILogger<HttpTrigger1> logger)
        {
            _logger = logger;
        }

        [Function("HttpTrigger1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            // Log all incoming headers
            foreach (var header in req.Headers)
            {
                _logger.LogInformation($"{header.Key}: {header.Value}");
            }

            // Build a response string with all headers
            var headersString = new StringBuilder();
            headersString.AppendLine("Hello and Welcome to Azure Functions!\n\nIncoming Headers:");
            foreach (var header in req.Headers)
            {
                headersString.AppendLine($"{header.Key}: {header.Value}");
            }

            _logger.LogTrace("TRACE===C# HTTP trigger function processed a request.");
            _logger.LogDebug("DEBUG===C# HTTP trigger function processed a request.");
            _logger.LogInformation("INFORMATION===C# HTTP trigger function processed a request.");
            _logger.LogWarning("WARNING===C# HTTP trigger function processed a request.");
            _logger.LogError("ERROR===C# HTTP trigger function processed a request.");
            _logger.LogCritical("CRITICAL===C# HTTP trigger function processed a request.");
            
            return new OkObjectResult(headersString.ToString());
        }
    }
}
