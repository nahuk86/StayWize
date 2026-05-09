using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using StayWize.Domain.Exceptions;
using StayWize.Services.Localization;
using StayWize.Services.Logging;

namespace StayWize.Services.ExceptionHandling;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogService _logService;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogService logService)
    {
        _next = next;
        _logService = logService;
    }

    public async Task InvokeAsync(HttpContext context, ILocalizationService localizationService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logService.LogError("Excepción no controlada: {Message}", ex, ex.Message);
            await HandleExceptionAsync(context, ex, localizationService);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILocalizationService localization)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            NotFoundException notFoundEx => new ErrorResponse
            {
                StatusCode = 404,
                Message = notFoundEx.EntityName is not null
                    ? localization.Get("NotFound", notFoundEx.EntityName)
                    : notFoundEx.Message
            },
            ConflictException conflictEx => new ErrorResponse
            {
                StatusCode = 409,
                Message = conflictEx.Message
            },
            ValidationException validationEx => new ErrorResponse
            {
                StatusCode = 400,
                Message = localization.Get("ValidationError"),
                Details = validationEx.Errors
            },
            ConcurrencyException => new ErrorResponse
            {
                StatusCode = 409,
                Message = localization.Get("ConcurrencyError")
            },
            _ => new ErrorResponse
            {
                StatusCode = 500,
                Message = localization.Get("InternalError")
            }
        };

        response.StatusCode = errorResponse.StatusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }
}