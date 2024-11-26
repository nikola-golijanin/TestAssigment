using Microsoft.Extensions.Options;
using System.Net;
using TestAssigmentAPI.Options;

namespace TestAssigmentAPI.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ClientsOptions _clientsOptions;


    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ClientsOptions> clientsOptions)
    {
        _next = next;
        _clientsOptions = clientsOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var userApiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("Api Key was not provided ");
            return;
        }

        var apiKey = _clientsOptions.Clients
            .Where(c => c.ApiKey == userApiKey)
            .Select(c => c.ApiKey)
            .FirstOrDefault();

        if (!IsApiKeyValid(userApiKey, apiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        await _next(context);
    }

    private static bool IsApiKeyValid(string? userApiKey, string? apiKey) =>
        userApiKey switch
        {
            _ when string.IsNullOrWhiteSpace(userApiKey) => false,
            _ when apiKey is null || apiKey != userApiKey => false,
            _ => true
        };
}

