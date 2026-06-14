using CSharpApp.Application.Categories;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var httpClientSettings = configuration
            .GetSection(nameof(HttpClientSettings))
            .Get<HttpClientSettings>() ?? new HttpClientSettings();

        var restApiSettings = configuration
            .GetSection(nameof(RestApiSettings))
            .Get<RestApiSettings>() ?? new RestApiSettings();

        Action<HttpClient> configureClient = client =>
        {
            client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        };

        services.AddHttpClient<IProductsService, ProductsService>(configureClient)
            .SetHandlerLifetime(TimeSpan.FromMinutes(httpClientSettings.LifeTime))
            .AddPolicyHandler(GetRetryPolicy(httpClientSettings))
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddHttpClient<ICategoriesService, CategoriesService>(configureClient)
            .SetHandlerLifetime(TimeSpan.FromMinutes(httpClientSettings.LifeTime))
            .AddPolicyHandler(GetRetryPolicy(httpClientSettings))
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    /// <summary>
    /// Uses RetryCount and SleepDuration from configuration
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(HttpClientSettings settings)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: settings.RetryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromMilliseconds(settings.SleepDuration * Math.Pow(2, retryAttempt - 1)));
    }

    /// <summary>
    /// Opens after 5 consecutive failures, stays open for 30 seconds
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDelay) =>
                {
                    // Circuit opened, API is failing, stop sending requests
                },
                onReset: () =>
                {
                    // Circuit closed - API is healthy again
                });
    }
}