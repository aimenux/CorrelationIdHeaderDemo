using CorrelationId;
using CorrelationId.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Example04.Tests;

public class CorrelationIdMiddlewareTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("123")]
    public async Task ShouldSetCorrelationIdToTraceIdentifier(string correlationId)
    {
        // arrange
        var logger = NullLogger<CorrelationIdMiddleware>.Instance;
        var options = Options.Create(new CorrelationIdOptions
        {
            IncludeInResponse = true,
            UpdateTraceIdentifier = true,
            RequestHeader = Constants.CorrelationIdHeaderName,
            ResponseHeader = Constants.CorrelationIdHeaderName,
            CorrelationIdGenerator = () => Guid.NewGuid().ToString()
        });
        var factory = Substitute.For<ICorrelationContextFactory>();
        var provider = Substitute.For<ICorrelationIdProvider>();
        provider
            .GenerateCorrelationId(Arg.Any<HttpContext>())
            .Returns(correlationId);
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        context.Request.Headers.Add(Constants.CorrelationIdHeaderName, correlationId);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next, logger, options, provider);

        // act
        await middleware.Invoke(context, factory);

        // assert
        context.TraceIdentifier.Should().Be(correlationId);
    }
}