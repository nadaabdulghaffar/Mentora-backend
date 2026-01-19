namespace Mentora.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
    {
        return services;
    }
}