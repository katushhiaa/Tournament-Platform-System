using System.Text;

namespace TournamentPlatformSystemWebApi.API.Middleware;

public class ResponseWrappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrappingMiddleware> _logger;

    public ResponseWrappingMiddleware(RequestDelegate next, ILogger<ResponseWrappingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip wrapping for health and swagger endpoints
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/health") || path.StartsWith("/swagger") || path.StartsWith("/swagger-ui"))
        {
            await _next(context);
            return;
        }

        var originalBody = context.Response.Body;
        var memStream = new MemoryStream();
        context.Response.Body = memStream;

        try
        {
            await _next(context);

            memStream.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(memStream, Encoding.UTF8).ReadToEndAsync();
            memStream.Seek(0, SeekOrigin.Begin);

            var contentType = context.Response.ContentType ?? string.Empty;
            var status = context.Response.StatusCode;

            if (status >= 200 && status < 300 && contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // Avoid double-wrapping if response already contains success field
                if (!bodyText.Contains("\"success\""))
                {
                    var wrapped = "{\"success\":true,\"data\":" + bodyText + "}";
                    var bytes = Encoding.UTF8.GetBytes(wrapped);
                    context.Response.ContentLength = bytes.Length;
                    context.Response.Body = originalBody;
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    return;
                }
            }

            // Fallback: copy original response through
            context.Response.Body = originalBody;
            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(context.Response.Body);
        }
        finally
        {
            // Ensure the original response stream is restored before disposing the temp stream
            context.Response.Body = originalBody;
            memStream.Dispose();
        }
    }
}
