using Microsoft.Extensions.DependencyInjection;
using StayWize.Services.Logging;

namespace StayWize.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogService, LogService>();
        return services;
    }
}