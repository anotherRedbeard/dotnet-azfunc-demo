using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

namespace Company.Function
{
    public class GraphMeEndpoint
    {
        private readonly ILogger<GraphMeEndpoint> _logger;
        private readonly HttpClient _httpClient;

        public GraphMeEndpoint(ILogger<GraphMeEndpoint> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        [Function("GraphMe")]
        [OpenApiOperation(operationId: "GraphMe", tags: new[] { "graph" }, Summary = "Get Graph /me", Description = "Calls Microsoft Graph API /me endpoint using the provided Authorization header")]
        [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for Microsoft Graph API")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "User profile from Microsoft Graph")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", bodyType: typeof(string), Description = "Missing or invalid Authorization header")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("GraphMe function triggered");

            // Get the Authorization header
            if (!req.Headers.TryGetValue("Authorization", out var authHeader))
            {
                _logger.LogWarning("No Authorization header provided");
                return new UnauthorizedObjectResult("Authorization header is required");
            }

            var token = authHeader.ToString();
            
            // Ensure it's a Bearer token
            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Authorization header is not a Bearer token");
                return new UnauthorizedObjectResult("Authorization header must be a Bearer token");
            }

            try
            {
                // Call Microsoft Graph /me endpoint
                var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);

                _logger.LogInformation("Calling Microsoft Graph /me endpoint");
                var response = await _httpClient.SendAsync(request);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Graph API returned {StatusCode}: {Content}", response.StatusCode, content);
                    return new ObjectResult(content)
                    {
                        StatusCode = (int)response.StatusCode
                    };
                }

                _logger.LogInformation("Successfully retrieved user profile from Graph API");
                return new ContentResult
                {
                    Content = content,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Microsoft Graph API");
                return new ObjectResult($"Error calling Graph API: {ex.Message}")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
