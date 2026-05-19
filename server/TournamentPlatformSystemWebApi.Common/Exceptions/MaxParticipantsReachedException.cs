using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class MaxParticipantsReachedException : Exception
    {
        public MaxParticipantsReachedException(string? message) : base(message) { }
    }
}
