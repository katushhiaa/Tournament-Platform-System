using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class MatchNotFoundException : Exception
    {
        public MatchNotFoundException(string? message) : base(message) { }
    }
}
