using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class TournamentClosedForChangesException : Exception
    {
        public TournamentClosedForChangesException(string? message) : base(message) { }
    }
}
