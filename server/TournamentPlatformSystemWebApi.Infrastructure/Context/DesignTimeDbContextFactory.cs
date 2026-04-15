using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TournamentPlatformSystemWebApi.Infrastructure.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TournamentdbContext>
{
    public TournamentdbContext CreateDbContext(string[] args)
    {
        try
        {
            string connectionString = string.Empty;

            var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"));
            if (File.Exists(envPath))
            {
                var envLines = File.ReadAllLines(envPath);
                var dict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var line in envLines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                        continue;
                    var idx = trimmed.IndexOf('=');
                    if (idx <= 0) continue;
                    var key = trimmed.Substring(0, idx).Trim();
                    var val = trimmed.Substring(idx + 1).Trim().Trim('"');
                    dict[key] = val;
                }


                var host = "localhost";
                var port = "5432";
                var database = dict.TryGetValue("POSTGRES_DB", out var n) ? n : string.Empty;
                var user = dict.TryGetValue("POSTGRES_USER", out var u) ? u : string.Empty;
                var password = dict.TryGetValue("POSTGRES_PASSWORD", out var pw) ? pw : string.Empty;
                connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password}";

            }

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException();

            var optionsBuilder = new DbContextOptionsBuilder<TournamentdbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            System.Console.WriteLine(connectionString);

            return new TournamentdbContext(optionsBuilder.Options);

        }
        catch
        {
            throw;
        }

    }
}
