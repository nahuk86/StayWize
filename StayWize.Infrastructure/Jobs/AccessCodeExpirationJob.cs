using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StayWize.Application.Common.Interfaces;
using StayWize.Services.Notifications;

namespace StayWize.Infrastructure.Jobs;

public class AccessCodeExpirationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccessCodeExpirationJob> _logger;
    private readonly TimeSpan _interval;

    public AccessCodeExpirationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<AccessCodeExpirationJob> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var minutes = int.Parse(
            configuration["JobSettings:AccessCodeExpirationIntervalMinutes"] ?? "15");
        _interval = TimeSpan.FromMinutes(minutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AccessCodeExpirationJob iniciado. Intervalo: {Interval} minutos.", _interval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessExpiredCodesAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessExpiredCodesAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var accessCodeRepository = scope.ServiceProvider.GetRequiredService<IAccessCodeRepository>();
            var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
            var propertyRepository = scope.ServiceProvider.GetRequiredService<IPropertyRepository>();
            var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var expiredCodes = await accessCodeRepository.GetExpiredActiveCodesAsync();
            var codesList = expiredCodes.ToList();

            if (!codesList.Any())
            {
                _logger.LogInformation("AccessCodeExpirationJob: no hay códigos para expirar.");
                return;
            }

            int processed = 0;

            foreach (var code in codesList)
            {
                try
                {
                    code.MarkAsExpired();
                    code.MarkAsUpdated("system");
                    await accessCodeRepository.UpdateAsync(code);

                    // Notificación al cliente
                    if (code.Reservation is not null)
                    {
                        var client = await clientRepository.GetByIdAsync(code.Reservation.ClientId);
                        var property = await propertyRepository.GetByIdAsync(code.Reservation.PropertyId);

                        if (client is not null && property is not null)
                        {
                            _ = emailService.SendAccessCodeExpiredAsync(
                                client.Email,
                                $"{client.FirstName} {client.LastName}",
                                property.Name);
                        }
                    }

                    processed++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al expirar código {CodeId}", code.Id);
                }
            }

            _logger.LogInformation("AccessCodeExpirationJob: {Count} códigos expirados.", processed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error general en AccessCodeExpirationJob.");
        }
    }
}