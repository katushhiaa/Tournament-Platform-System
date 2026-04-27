using System;
using TournamentPlatformSystemWebApi.Core.Common;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IDbStateChecker
{
    Task<DbPingResult> PingAsync(CancellationToken cancellationToken = default);
}
