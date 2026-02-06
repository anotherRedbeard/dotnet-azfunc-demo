using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
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
        [OpenApiOperation(operationId: "HttpTrigger1", tags: new[] { "greeting" }, Summary = "HTTP Trigger Function", Description = "Returns a welcome message with incoming HTTP headers")]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Optional name parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Successful response with headers information")]
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
