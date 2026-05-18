namespace StayWize.Application.Common.Interfaces;

public interface ISmartLockService
{
    /// <summary>
    /// Configura un código de acceso en la cerradura IoT para el período indicado.
    /// </summary>
    Task<SmartLockResult> SetCodeAsync(string lockDeviceId, string code, DateTime validFrom, DateTime validTo);

    /// <summary>
    /// Revoca un código de acceso de la cerradura IoT.
    /// </summary>
    Task<SmartLockResult> RevokeCodeAsync(string lockDeviceId, string code);
}

public record SmartLockResult(bool Success, string? ErrorMessage = null)
{
    public static SmartLockResult Ok() => new(true);
    public static SmartLockResult Fail(string errorMessage) => new(false, errorMessage);
}
