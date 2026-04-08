
using TournamentPlatformSystemWebApi.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration moved to Swagger/SwaggerExtensions.cs
builder.Services.AddPlayTourSwagger();

var app = builder.Build();

// Use swagger middleware from extensions
app.UsePlayTourSwagger();

app.MapControllers();

app.Run();

