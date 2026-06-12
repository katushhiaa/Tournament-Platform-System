using System.Diagnostics;

namespace TournamentPlatformSystemWebApi.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var status = context.Response?.StatusCode;
            _logger.LogInformation("{Method} {Path} responded {Status} in {Elapsed}ms",
                context.Request?.Method,
                context.Request?.Path,
                status,
                sw.ElapsedMilliseconds);
        }
    }
}
