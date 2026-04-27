using System;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public abstract class BaseDbEntity
{
    public Guid Id { get; set; }
}

