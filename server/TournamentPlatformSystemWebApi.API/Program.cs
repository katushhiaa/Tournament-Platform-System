
using TournamentPlatformSystemWebApi.API.Swagger;
using TournamentPlatformSystemWebApi.API.Middleware;
using Microsoft.AspNetCore.Mvc;
using TournamentPlatformSystemWebApi.API.Endpoints;
using TournamentPlatformSystemWebApi.Infrastructure.Services;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Models;
using TournamentPlatformSystemWebApi.Infrastructure.Configurations;
using TournamentPlatformSystemWebApi.Infrastructure.Repositories;
using TournamentPlatformSystemWebApi.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Load optional secrets file from project root (useful in Docker or local mounts)
// The file should be named `tournament-platform-system-secrets.json` and contain valid JSON
// matching the expected configuration structure).
var secretsFile = "tournament-platform-system-secrets.json";
if (File.Exists(secretsFile))
{
    builder.Configuration.AddJsonFile(secretsFile, optional: true, reloadOnChange: false);
}
else
{
    System.Console.WriteLine("`tournament-platform-system-secrets.json` not provided");
    builder.Configuration.AddJsonFile(secretsFile, optional: true, reloadOnChange: false);
}

// Add services to the container.
builder.Services.AddControllers();

// Add in-memory cache for rate-limiting login attempts
builder.Services.AddMemoryCache();

// Return validation errors in unified ErrorResponseDto format
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var fieldErrors = context.ModelState
            .Where(x => x.Value != null && x.Value.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value!.Errors.Select(e => new FieldError
            {
                Field = string.IsNullOrEmpty(kvp.Key) ? "" : char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                Message = e.ErrorMessage
            }))
            .ToList();

        var errorDto = new ErrorResponseDto
        {
            Error = new ErrorDetail
            {
                Code = StatusCodes.Status400BadRequest,
                Type = "ValidationError",
                Message = "Validation failed",
                FieldErrors = fieldErrors,
                Path = context.HttpContext.Request.Path,
                Timestamp = DateTime.UtcNow.ToString("o"),
                TraceId = context.HttpContext.TraceIdentifier
            }
        };

        return new BadRequestObjectResult(errorDto);
    };
});
builder.Services.AddEndpointsApiExplorer();

// Configure CORS to allow requests from frontend origin (set via configuration or env var)
var frontendOrigin = Environment.GetEnvironmentVariable("FRONTEND_ORIGIN") ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>(sp => new BcryptPasswordHasher(12));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
// JWT token options and service
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = new JwtTokenOptions(
    jwtSection["Key"] ?? "default-development-key-change-in-production",
    jwtSection["Issuer"] ?? "tournament-api",
    int.TryParse(jwtSection["ExpirationMinutes"], out var m) ? m : 10
);

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>(sp => new JwtTokenService(jwtOptions));

// Register tournament service
builder.Services.AddScoped<ITournamentService, TournamentService>();

// Register storage service (Google Drive)
builder.Services.AddSingleton<IStorageService, GoogleDriveStorageService>();

// Configure JWT authentication and set DefaultChallengeScheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.Key ?? "default-development-key-change-in-production");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer ?? "tournament-api",
        ValidateAudience = true,
        ValidAudience = jwtOptions.Issuer ?? "tournament-api",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        // Use the "role" claim emitted by JwtTokenService for role checks
        RoleClaimType = ClaimTypes.Role,
        // Use subject as name identifier
        NameClaimType = JwtRegisteredClaimNames.Sub
    };

    //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI4NzU2MDI1MC01N2UyLTQ4OTgtYWUxMi03OTU1ZTYwMWIxYmQiLCJlbWFpbCI6Im9yZ2FuaXplckBleGFtcGxlLmNvbSIsInJvbGUiOiJvcmdhbml6ZXIiLCJpc09yZ2FuaXplciI6IlRydWUiLCJleHAiOjE3Nzc5MjI1OTIsImlzcyI6InRvdXJuYW1lbnQtYXBpIiwiYXVkIjoidG91cm5hbWVudC1hcGkifQ._lfSPLKLpbs_L5PFEEuVt_CvwABmgdGnN459DS-jb7c
});

builder.Services.AddAuthorization();

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

// Enable CORS using policy that allows the frontend origin
app.UseCors("FrontendPolicy");

// Health endpoint for liveness/readiness checks
app.MapGet("health", HealthHandler.HandleAsync);

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
