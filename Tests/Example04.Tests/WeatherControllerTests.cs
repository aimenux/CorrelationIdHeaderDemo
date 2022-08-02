using System.Net;
using FluentAssertions;

namespace Example04.Tests;

public class WeatherControllerTests
{
    [Theory]
    [InlineData("xyz")]
    [InlineData("123")]
    public async Task ShouldGetWeatherInfoReturnOk(string correlationId)
    {
        // arrange
        var weatherInfo = new WeatherInfo
        {
            Wind = "15 km/h",
            Temperature = "+38 °C",
            Description = "Sunny"
        };

        var fixture = new WebApiTestFixture(weatherInfo);

        var client = fixture.CreateClientWithCorrelationIdHeader(correlationId);

        // act
        var response = await client.GetAsync("/weather/known-city");
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseHeaders = response.Headers.ToDictionary(a => a.Key, a => a.Value);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().Be("{\"description\":\"Sunny\",\"temperature\":\"+38 °C\",\"wind\":\"15 km/h\"}");
        responseHeaders.Should().ContainKey(Constants.CorrelationIdHeaderName);
        responseHeaders[Constants.CorrelationIdHeaderName].Should().BeEquivalentTo(correlationId);
    }

    [Fact]
    public async Task ShouldGetWeatherInfoReturnNotFound()
    {
        // arrange
        var fixture = new WebApiTestFixture(null);

        var client = fixture.CreateClient();

        // act
        var response = await client.GetAsync("/weather/unknown-city");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}