using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Models;

namespace TournamentPlatformSystemWebApi.API.Endpoints
{
    public static class HealthHandler
    {
        public static async Task HandleAsync(HttpContext context, IDbStateChecker dbStateChecker)
        {
            context.Response.Clear();

            var dbPingResult = await dbStateChecker.PingAsync();

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            if (!dbPingResult.IsUp)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.ContentType = "application/json";

                var errorDto = new ErrorResponseDto()
                {
                    Error = new ErrorDetail
                    {
                        Code = (int)HttpStatusCode.ServiceUnavailable,
                        Type = "ServiceUnavailable",
                        Message = dbPingResult.ErrorMessage,
                        Path = context.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = context.TraceIdentifier
                    }
                };

                var errorJson = JsonSerializer.Serialize(errorDto, options);
                await context.Response.WriteAsync(errorJson);
                return;
            }

            var okObj = new
            {
                success = true,
                timestamp = DateTime.UtcNow.ToString("o"),
                databaseState = dbPingResult.State
            };

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(okObj, options));
        }
    }
}
