namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
    public static IServiceCollection AddDefaultConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RestApiSettings>(configuration.GetSection(nameof(RestApiSettings)));
        services.Configure<HttpClientSettings>(configuration.GetSection(nameof(HttpClientSettings)));

        // Register MediatR and scan for handlers in Application assembly
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(ProductsService).Assembly));

        return services;
    }
}