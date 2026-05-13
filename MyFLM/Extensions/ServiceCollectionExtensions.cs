using Microsoft.Extensions.DependencyInjection;

namespace MyFLM.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to keep Program.cs clean.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add business services, validators, mappings here

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Add DB Context, UnitOfWork, external service adapters here

        return services;
    }
}