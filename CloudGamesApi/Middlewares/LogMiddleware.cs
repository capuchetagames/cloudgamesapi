using Microsoft.Extensions.Primitives;

namespace CloudGamesApi.Middlewares;

public class LogMiddleware
{
    private readonly RequestDelegate _next;
    private const string _correlationIdHeader = "X-Correlation-ID";

    public LogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        AddCorrelationIdHeaderToResponse(context, correlationId);
        
        
        return _next(context);
    }
    private StringValues GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationId))
        {
            return correlationId;
        }
        
        correlationId = Guid.NewGuid().ToString();
        return correlationId;
    }
    private void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[_correlationIdHeader] = new []{correlationId.ToString()};
            return Task.CompletedTask;
        });
    }
}

public static class LogMiddlewareExtensions
{
    public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LogMiddleware>();
    }
}