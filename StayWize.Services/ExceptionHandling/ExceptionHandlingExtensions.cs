using Microsoft.AspNetCore.Builder;

namespace StayWize.Services.ExceptionHandling;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}