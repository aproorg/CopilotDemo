using CopilotDemo.Web.Configuration;
using Microsoft.Extensions.Options;

namespace CopilotDemo.Web.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IOptions<ApiKeyOptions> apiKeyOptions)
    {
        // Skip authentication for Swagger endpoints
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var options = apiKeyOptions.Value;
        
        if (!context.Request.Headers.TryGetValue(options.HeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("API Key was not provided in header: {HeaderName}", options.HeaderName);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        if (!options.Key.Equals(extractedApiKey))
        {
            _logger.LogWarning("Unauthorized API Key attempt");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client");
            return;
        }

        await _next(context);
    }
}
