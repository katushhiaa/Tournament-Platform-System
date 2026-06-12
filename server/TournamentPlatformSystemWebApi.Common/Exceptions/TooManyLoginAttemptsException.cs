using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class TooManyLoginAttemptsException : Exception
    {
        public TooManyLoginAttemptsException(string? message) : base(message) { }
    }
}
