
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Common;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

public class DbStateChecker : IDbStateChecker
{
    private readonly string _connectionString;

    public DbStateChecker(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DbPingResult> PingAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            return new DbPingResult(IsUp: false, State: ConnectionState.Closed.ToString(), ErrorMessage: "Connection string is empty");
        }

        try
        {
            await using var ds = NpgsqlDataSource.Create(_connectionString);
            await using var conn = await ds.OpenConnectionAsync(cancellationToken);
            var state = conn.FullState.ToString();
            await conn.CloseAsync();
            return new DbPingResult(IsUp: true, State: state, ErrorMessage: null);
        }
        catch (Exception ex)
        {
            return new DbPingResult(IsUp: false, State: ConnectionState.Closed.ToString(), ErrorMessage: ex.Message);
        }
    }
}
