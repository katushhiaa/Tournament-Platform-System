using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string? message) : base(message) { }
    }
}
