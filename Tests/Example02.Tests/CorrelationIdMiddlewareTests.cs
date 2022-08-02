using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Example02.Tests;

public class CorrelationIdMiddlewareTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("123")]
    public async Task ShouldSetCorrelationIdToTraceIdentifier(string correlationId)
    {
        // arrange
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        context.Request.Headers.Add(Constants.CorrelationIdHeaderName, correlationId);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.TraceIdentifier.Should().Be(correlationId);
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData("123")]
    public async Task ShouldAddCorrelationIdToResponseHeaders(string correlationId)
    {
        // arrange
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        context.Request.Headers.Add(Constants.CorrelationIdHeaderName, correlationId);

        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // act
        await middleware.InvokeAsync(context);

        // assert
        context.Response.Headers.Should().ContainKey(Constants.CorrelationIdHeaderName);
        context.Response.Headers[Constants.CorrelationIdHeaderName].Should().BeEquivalentTo(correlationId);
    }
}