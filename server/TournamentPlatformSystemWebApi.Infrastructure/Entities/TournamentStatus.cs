using System;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public enum TournamentStatusType
{
    REGISTRATION_OPEN = 0,
    REGISTRATION_CLOSED,
    IN_PROGRESS,
    COMPLETED
}
