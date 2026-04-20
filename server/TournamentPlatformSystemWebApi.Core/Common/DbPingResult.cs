using System;

namespace TournamentPlatformSystemWebApi.Core.Common;

public record DbPingResult(bool IsUp, string? ErrorMessage, string? State);
