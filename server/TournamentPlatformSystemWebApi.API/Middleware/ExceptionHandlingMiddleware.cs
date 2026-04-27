using System.Net;
using System.Text.Json;
using TournamentPlatformSystemWebApi.Common.Models;
using Microsoft.AspNetCore.Http;

namespace TournamentPlatformSystemWebApi.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorObj = new ErrorResponseDto()
            {
                Error = new ErrorDetail
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Type = "InternalServerError",
                    Message = ex.Message,
                    Path = context.GetEndpoint().DisplayName,
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TraceId = context.TraceIdentifier
                }
            };

            var json = JsonSerializer.Serialize(errorObj);
            await context.Response.WriteAsync(json);
        }
    }
}
