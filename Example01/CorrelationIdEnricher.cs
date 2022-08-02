using Serilog.Core;
using Serilog.Events;

namespace Example01;

public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var correlationId = _httpContextAccessor.HttpContext?.TraceIdentifier;
        var property = propertyFactory.CreateProperty(Constants.CorrelationIdLoggingName, correlationId);
        logEvent.AddPropertyIfAbsent(property);
    }
}