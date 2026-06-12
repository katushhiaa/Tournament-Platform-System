using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class TournamentAlreadyStartedException : Exception
    {
        public TournamentAlreadyStartedException(string? message) : base(message) { }
    }
}
