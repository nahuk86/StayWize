using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StayWize.Services.Authentication;
using StayWize.Services.Encryption;
using StayWize.Services.Localization;
using StayWize.Services.Logging;
using StayWize.Services.Notifications;
using System.Text;

namespace StayWize.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Logging
        services.AddSingleton<ILogService, LogService>();

        // Localization
        services.AddHttpContextAccessor();
        services.AddScoped<ILocalizationService, LocalizationService>();

        // Encryption
        services.AddSingleton<IEncryptionService, EncryptionService>();

        // Auth
        services.AddScoped<IAuthService, AuthService>();

        // Notifications
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IUserService, UserService>();


        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey))
            };
        });

        services.AddAuthorization();

        return services;
    }
}