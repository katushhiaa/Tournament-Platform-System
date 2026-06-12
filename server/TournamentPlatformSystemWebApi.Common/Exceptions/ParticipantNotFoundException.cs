using System;

namespace TournamentPlatformSystemWebApi.Common.Exceptions
{
    public class ParticipantNotFoundException : Exception
    {
        public ParticipantNotFoundException(string? message) : base(message) { }
    }
}
