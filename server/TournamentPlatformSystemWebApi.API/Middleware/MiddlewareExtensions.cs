using Microsoft.AspNetCore.Builder;

namespace TournamentPlatformSystemWebApi.API.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }

    public static IApplicationBuilder UseResponseWrapping(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResponseWrappingMiddleware>();
    }
}
