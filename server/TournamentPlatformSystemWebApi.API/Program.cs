
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
using TournamentPlatformSystemWebApi.Infrastructure.Configurations;
using TournamentPlatformSystemWebApi.Infrastructure.Repositories;
using TournamentPlatformSystemWebApi.Infrastructure.Security;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add db repositories and state checker
var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
// configure db
builder.Services.AddTournamentDb(dbConnectionString);

builder.Services.AddTransient<IDbStateChecker, DbStateChecker>(serviceProvider =>
{
    return new DbStateChecker(dbConnectionString);
});

// infra services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>(sp => new BcryptPasswordHasher(12));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
// JWT token options and service
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = new JwtTokenOptions(
    jwtSection["Key"] ?? "default-development-key-change-in-production",
    jwtSection["Issuer"] ?? "tournament-api",
    int.TryParse(jwtSection["ExpirationDays"], out var d) ? d : 1
);

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>(sp => new JwtTokenService(jwtOptions));

// Swagger configuration moved to Swagger/SwaggerExtensions.cs
builder.Services.AddConfiguredSwagger();
// Register AutoMapper profiles from Infrastructure assembly
builder.Services.AddAutoMapper(typeof(TournamentPlatformSystemWebApi.Infrastructure.Mappings.UserProfile).Assembly);

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
