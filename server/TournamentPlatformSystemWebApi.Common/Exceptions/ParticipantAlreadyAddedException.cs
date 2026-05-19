using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class ParticipantAlreadyAddedException : Exception
    {
        public ParticipantAlreadyAddedException(string? message) : base(message) { }
    }
}
