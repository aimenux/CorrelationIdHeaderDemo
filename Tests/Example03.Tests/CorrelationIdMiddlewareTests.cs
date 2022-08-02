using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Example03.Tests;

public class CorrelationIdMiddlewareTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("123")]
    public async Task ShouldSetCorrelationIdToTraceIdentifier(string correlationId)
    {
        // arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(ICorrelationIdAccessor))
            .Returns(new CorrelationIdAccessor());
        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
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
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(ICorrelationIdAccessor))
            .Returns(new CorrelationIdAccessor());
        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
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