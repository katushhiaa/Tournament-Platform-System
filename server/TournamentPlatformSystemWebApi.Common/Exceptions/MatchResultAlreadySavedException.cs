using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class MatchResultAlreadySavedException : Exception
    {
        public MatchResultAlreadySavedException(string? message) : base(message) { }
    }
}
