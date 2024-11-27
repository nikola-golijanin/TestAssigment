using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using TestAssigmentAPI.Options;

namespace TestAssigmentAPI.Middleware;

public class RequestLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestLoggingFilter> _logger;
    private readonly ClientsOptions _clientsOptions;
    private const string ApiKeyHeaderName = "X-API-Key";


    public RequestLoggingFilter(
        ILogger<RequestLoggingFilter> logger,
        IOptions<ClientsOptions> clientsOptions)
    {
        _logger = logger;
        _clientsOptions = clientsOptions.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;
        var callerIp = httpContext.Connection.RemoteIpAddress?.ToString();
        var hostName = httpContext.Request.Host.Value;
        var requestParams = string.Join(", ", request.Query.Select(q => $"{q.Key}={q.Value}"));

        if (httpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var userApiKey))
        {
            var clientInfo = _clientsOptions.Clients
                .FirstOrDefault(c => c.ApiKey == userApiKey);

            _logger.LogInformation("Incoming request: {Method} {Path}, clientInfo: {Client}, callerIp: {CallerIp}, hostName: {HostName}, requestParams: {RequestParams}",
                request.Method,
                request.Path,
                clientInfo,
                callerIp,
                hostName,
                requestParams);
        }
        else
        {
            _logger.LogInformation("Incoming request: {Method} {Path}, callerIp: {CallerIp}, hostName: {HostName}, requestParams: {RequestParams}",
                request.Method,
                request.Path,
                callerIp,
                hostName,
                requestParams);
        }

        await next();
    }
}
