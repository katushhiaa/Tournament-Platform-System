using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions;

public class DuplicateTournamentTitleException : Exception
{
    public DuplicateTournamentTitleException(string? message) : base(message)
    {

    }
}
