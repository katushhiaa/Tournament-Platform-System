using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class InsufficientParticipantsException : Exception
    {
        public InsufficientParticipantsException(string? message) : base(message) { }
    }
}
