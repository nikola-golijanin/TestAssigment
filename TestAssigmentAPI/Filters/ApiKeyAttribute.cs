using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using TestAssigmentAPI.Options;

namespace TestAssigmentAPI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!IsApiKeyValid(context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private static bool IsApiKeyValid(HttpContext context)
    {
        string? apiKey = context.Request.Headers[ApiKeyHeaderName];
        if (string.IsNullOrWhiteSpace(apiKey)) return false;

        var clients = context.RequestServices
                        .GetRequiredService<IOptions<ClientsOptions>>()
                        .Value
                        .Clients;

        return clients.Any(c => c.ApiKey == apiKey);
    }
}
