
using TournamentPlatformSystemWebApi.API.Swagger;
using TournamentPlatformSystemWebApi.API.Middleware;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TournamentPlatformSystemWebApi.API.Endpoints;
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
// Register AutoMapper profiles from Infrastructure assembly
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.UserProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.MatchProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.StatusProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.TeamProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.TournamentProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.UserTeamProfile).Assembly);

var app = builder.Build();

// Register middlewares: logging -> exception handling -> response wrapping
app.UseRequestLogging();
app.UseExceptionHandling();
app.UseResponseWrapping();

// Use swagger middleware from extensions
app.UseConfiguredSwagger();

// Health endpoint for liveness/readiness checks
app.MapGet("health", HealthHandler.HandleAsync);

app.MapControllers();

app.Run();
