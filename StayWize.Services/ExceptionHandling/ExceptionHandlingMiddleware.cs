using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using StayWize.Domain.Exceptions;
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

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logService.LogError("Excepción no controlada: {Message}", ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            AppException appEx => new ErrorResponse
            {
                StatusCode = appEx.StatusCode,
                Message = appEx.Message,
                Details = appEx is ValidationException validationEx
                    ? validationEx.Errors
                    : null
            },
            ConcurrencyException concurrencyEx => new ErrorResponse
            {
                StatusCode = 409,
                Message = concurrencyEx.Message
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Ocurrió un error interno. Intentá nuevamente más tarde."
            }
        };

        response.StatusCode = errorResponse.StatusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }
}