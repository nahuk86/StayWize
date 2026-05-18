using Microsoft.Extensions.Logging;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Infrastructure.Services;

/// <summary>
/// Implementación stub de ISmartLockService para desarrollo y testing.
/// Loguea las operaciones y simula éxito sin conectarse a hardware real.
/// Reemplazar por la implementación del proveedor de cerraduras en producción.
/// </summary>
public class StubSmartLockService : ISmartLockService
{
    private readonly ILogger<StubSmartLockService> _logger;

    public StubSmartLockService(ILogger<StubSmartLockService> logger)
    {
        _logger = logger;
    }

    public Task<SmartLockResult> SetCodeAsync(
        string lockDeviceId, string code, DateTime validFrom, DateTime validTo)
    {
        _logger.LogInformation(
            "[SmartLock STUB] SetCode → Device: {DeviceId} | Code: {Code} | From: {From:dd/MM/yyyy HH:mm} | To: {To:dd/MM/yyyy HH:mm}",
            lockDeviceId, code, validFrom, validTo);

        return Task.FromResult(SmartLockResult.Ok());
    }

    public Task<SmartLockResult> RevokeCodeAsync(string lockDeviceId, string code)
    {
        _logger.LogInformation(
            "[SmartLock STUB] RevokeCode → Device: {DeviceId} | Code: {Code}",
            lockDeviceId, code);

        return Task.FromResult(SmartLockResult.Ok());
    }
}
