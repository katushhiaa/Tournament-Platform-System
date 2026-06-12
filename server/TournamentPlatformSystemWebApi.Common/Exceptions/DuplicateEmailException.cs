using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string? message) : base(message) { }
    }
}
