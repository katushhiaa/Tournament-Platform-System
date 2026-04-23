using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Configurations;

public static class TournamentDbDataSourceConfiguring
{
    public static void AddTournamentDb(this IServiceCollection services, string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        dataSourceBuilder.MapEnum<TournamentStatusType>("tournament_status");

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<TournamentdbContext>(options =>
            options.UseNpgsql(dataSource));
    }
}
