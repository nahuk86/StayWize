using Microsoft.Extensions.DependencyInjection;
using StayWize.Application.Common.Services;

namespace StayWize.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Singleton porque los locks deben vivir toda la vida de la aplicación
        services.AddSingleton<ReservationConcurrencyService>();

        return services;
    }
}