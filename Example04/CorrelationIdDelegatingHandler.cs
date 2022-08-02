using CorrelationId.Abstractions;

namespace Example04;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly ICorrelationContextAccessor _correlationContextAccessor;

    public CorrelationIdDelegatingHandler(ICorrelationContextAccessor correlationContextAccessor)
    {
        _correlationContextAccessor = correlationContextAccessor ?? throw new ArgumentNullException(nameof(correlationContextAccessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = _correlationContextAccessor.CorrelationContext.CorrelationId;
        request.Headers.Add(Constants.CorrelationIdHeaderName, correlationId);
        return await base.SendAsync(request, cancellationToken);
    }
}