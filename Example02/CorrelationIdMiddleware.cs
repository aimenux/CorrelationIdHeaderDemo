using Serilog.Context;

namespace Example02;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ExtractOrComputeCorrelationId(context);
        context.TraceIdentifier = correlationId;
        context.Request.Headers.TryAdd(Constants.CorrelationIdHeaderName, correlationId);
        context.Response.Headers.TryAdd(Constants.CorrelationIdHeaderName, correlationId);
        using (LogContext.PushProperty(Constants.CorrelationIdLoggingName, correlationId))
        {
            await _next(context);
        }
    }

    private static string ExtractOrComputeCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(Constants.CorrelationIdHeaderName, out var correlationId) && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId;
        }

        return Guid.NewGuid().ToString();
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}