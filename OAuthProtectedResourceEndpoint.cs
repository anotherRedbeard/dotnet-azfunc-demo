using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Company.Function
{
    public class OAuthProtectedResourceEndpoint
    {
        private readonly ILogger<OAuthProtectedResourceEndpoint> _logger;

        public OAuthProtectedResourceEndpoint(ILogger<OAuthProtectedResourceEndpoint> logger)
        {
            _logger = logger;
        }

        [Function("OAuthProtectedResource")]
        [OpenApiOperation(operationId: "OAuthProtectedResource", tags: new[] { "oauth" }, Summary = "OAuth Protected Resource Metadata", Description = "Returns OAuth 2.0 Protected Resource Metadata")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "Protected resource metadata")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/oauth-protected-resource")] HttpRequest req)
        {
            _logger.LogInformation("OAuth Protected Resource metadata requested");

            return new OkObjectResult(new { });
        }
    }
}
