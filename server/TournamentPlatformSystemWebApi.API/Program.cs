
using TournamentPlatformSystemWebApi.API.Swagger;
using TournamentPlatformSystemWebApi.API.Middleware;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TournamentPlatformSystemWebApi.Infrastructure.Services;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add db repositories and state checker
var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

builder.Services.AddTransient<IDbStateChecker, DbStateChecker>(serviceProvider =>
{
    return new DbStateChecker(dbConnectionString);
});

// Swagger configuration moved to Swagger/SwaggerExtensions.cs
builder.Services.AddConfiguredSwagger();

var app = builder.Build();

// Register middlewares: logging -> exception handling -> response wrapping
app.UseRequestLogging();
app.UseExceptionHandling();
app.UseResponseWrapping();

// Use swagger middleware from extensions
app.UseConfiguredSwagger();

// Health endpoint for liveness/readiness checks
app.MapGet("health", HealthHandler);

async Task HealthHandler(HttpContext context)
{
    IDbStateChecker dbStateChecker = app.Services.GetRequiredService<IDbStateChecker>();

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
                Path = context.GetEndpoint().DisplayName,
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

app.MapControllers();

app.Run();
