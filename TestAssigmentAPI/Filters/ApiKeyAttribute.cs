﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using TestAssigmentAPI.Options;

namespace TestAssigmentAPI.Middleware;

internal sealed class ApiKeyAttribute : IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    private readonly ClientsOptions _clientsOptions;

    public ApiKeyAttribute(IOptions<ClientsOptions> clientsOptions)
    {
        _clientsOptions = clientsOptions.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        if (!httpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var userApiKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var apiKey = _clientsOptions.Clients
            .Where(c => c.ApiKey == userApiKey)
            .Select(c => c.ApiKey)
            .FirstOrDefault();

        if (!IsApiKeyValid(userApiKey, apiKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }

    private static bool IsApiKeyValid(string? userApiKey, string? apiKey) =>
        userApiKey switch
        {
            _ when string.IsNullOrWhiteSpace(userApiKey) => false,
            _ when apiKey is null || apiKey != userApiKey => false,
            _ => true
        };
}