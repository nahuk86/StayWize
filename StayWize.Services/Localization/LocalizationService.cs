using Microsoft.AspNetCore.Http;

namespace StayWize.Services.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly Dictionary<string, Dictionary<string, string>> _messages = new()
    {
        ["es"] = new()
        {
            ["NotFound"] = "{0} no fue encontrado.",
            ["Conflict"] = "Ya existe un registro con esos datos.",
            ["ValidationError"] = "Se produjeron errores de validación.",
            ["ConcurrencyError"] = "Conflicto de concurrencia. Intentá nuevamente.",
            ["InternalError"] = "Error interno del servidor.",
            ["InvalidCredentials"] = "Credenciales inválidas.",
            ["ReservationOverlap"] = "La propiedad ya tiene una reserva en ese rango de fechas.",
            ["AccessCodeInvalid"] = "Código de acceso inválido."
        },
        ["en"] = new()
        {
            ["NotFound"] = "{0} was not found.",
            ["Conflict"] = "A record with that data already exists.",
            ["ValidationError"] = "One or more validation errors occurred.",
            ["ConcurrencyError"] = "Concurrency conflict. Please try again.",
            ["InternalError"] = "Internal server error.",
            ["InvalidCredentials"] = "Invalid credentials.",
            ["ReservationOverlap"] = "The property already has a reservation for that date range.",
            ["AccessCodeInvalid"] = "Invalid access code."
        }
    };

    public LocalizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCulture()
    {
        var acceptLanguage = _httpContextAccessor.HttpContext?
            .Request.Headers["Accept-Language"].ToString();

        if (!string.IsNullOrEmpty(acceptLanguage) && acceptLanguage.StartsWith("en"))
            return "en";

        return "es";
    }

    public string Get(string key)
    {
        var culture = GetCulture();
        if (_messages.TryGetValue(culture, out var dict) && dict.TryGetValue(key, out var value))
            return value;
        return key;
    }

    public string Get(string key, params object[] args)
    {
        var message = Get(key);
        return string.Format(message, args);
    }
}