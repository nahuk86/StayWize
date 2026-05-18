using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StayWize.Application.Common.Interfaces;
using StayWize.Infrastructure.Jobs;
using StayWize.Infrastructure.Persistence.Context;
using StayWize.Infrastructure.Persistence.Repositories;
using StayWize.Infrastructure.Services;
using StayWize.Services.Authentication;

namespace StayWize.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IHostLocalRepository, HostLocalRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IAccessCodeRepository, AccessCodeRepository>();
        services.AddScoped<IAccessLogRepository, AccessLogRepository>();
        services.AddScoped<IUserInvitationRepository, UserInvitationRepository>();

        // IoT: stub reemplazable por implementación real del proveedor de cerraduras
        services.AddScoped<ISmartLockService, StubSmartLockService>();

        services.AddHostedService<AccessCodeExpirationJob>();

        return services;
    }
}
