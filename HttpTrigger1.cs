using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
            _logger.LogTrace("TRACE===C# HTTP trigger function processed a request.");
            _logger.LogDebug("DEBUG===C# HTTP trigger function processed a request.");
            _logger.LogInformation("INFORMATION===C# HTTP trigger function processed a request.");
            _logger.LogWarning("WARNING===C# HTTP trigger function processed a request.");
            _logger.LogError("ERROR===C# HTTP trigger function processed a request.");
            _logger.LogCritical("CRITICAL===C# HTTP trigger function processed a request.");
            return new OkObjectResult("Hello and Welcome to Azure Functions!");
        }
    }
}
