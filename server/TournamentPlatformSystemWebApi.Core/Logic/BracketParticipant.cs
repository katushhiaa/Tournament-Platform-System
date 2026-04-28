using System;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public sealed class BracketParticipant
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int? Seed { get; init; }
        public int? Rating { get; init; }
    }
}
