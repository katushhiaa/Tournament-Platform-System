using System;
using System.Collections.Generic;
using System.Linq;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public sealed class RandomSeedingStrategy : ISeedingStrategy
    {
        private readonly Random _rng;

        public RandomSeedingStrategy(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public IReadOnlyList<BracketParticipant> Seed(IReadOnlyList<BracketParticipant> participants)
        {
            if (participants == null) return Array.Empty<BracketParticipant>();
            return participants.OrderBy(_ => _rng.Next()).ToList();
        }
    }
}
