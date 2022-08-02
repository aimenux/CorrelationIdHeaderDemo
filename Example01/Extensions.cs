using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Debugging;

namespace Example01;

public static class Extensions
{
    public static void AddDependencies(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IWeatherService, WeatherService>((provider, client) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var url = configuration.GetValue<string>("WeatherApi:BaseUrl");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.BaseAddress = new Uri(url);
        }).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

        serviceCollection.AddScoped<CorrelationIdDelegatingHandler>();

        serviceCollection.AddTransient<CorrelationIdEnricher>();

        serviceCollection.AddHttpContextAccessor();
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((hostingContext, serviceProvider, loggerConfiguration) =>
        {
            SelfLog.Enable(Console.Error);

            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.With(serviceProvider.GetRequiredService<CorrelationIdEnricher>());
        });

        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add(Constants.CorrelationIdHeaderName);
            logging.ResponseHeaders.Add(Constants.CorrelationIdHeaderName);
        });

        return builder;
    }

    public static void UseRequestResponseLogging(this IApplicationBuilder app)
    {
        app.UseWhen(IsLoggingEnabled, builder =>
        {
            builder.UseHttpLogging();
        });

        static bool IsLoggingEnabled(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            return !path.Contains("swagger", StringComparison.OrdinalIgnoreCase);
        }
    }
}