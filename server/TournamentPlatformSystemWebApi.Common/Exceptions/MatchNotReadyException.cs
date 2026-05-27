using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class MatchNotReadyException : Exception
    {
        public MatchNotReadyException(string? message) : base(message) { }
    }
}
