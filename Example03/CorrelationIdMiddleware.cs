using Serilog.Context;

namespace Example03;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var accessor = context.RequestServices.GetRequiredService<ICorrelationIdAccessor>();
        accessor.CorrelationId = ExtractOrComputeCorrelationId(context);
        context.TraceIdentifier = accessor.CorrelationId;
        context.Request.Headers.TryAdd(Constants.CorrelationIdHeaderName, accessor.CorrelationId);
        context.Response.Headers.TryAdd(Constants.CorrelationIdHeaderName, accessor.CorrelationId);
        using (LogContext.PushProperty(Constants.CorrelationIdLoggingName, accessor.CorrelationId))
        {
            await _next(context);
        }
    }

    private static string ExtractOrComputeCorrelationId(HttpContext context)
    {
        if (context is null) return null;

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