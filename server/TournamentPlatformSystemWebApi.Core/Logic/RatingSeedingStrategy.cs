using System.Collections.Generic;
using System.Linq;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public sealed class RatingSeedingStrategy : ISeedingStrategy
    {
        public IReadOnlyList<BracketParticipant> Seed(IReadOnlyList<BracketParticipant> participants)
        {
            if (participants == null) return new List<BracketParticipant>();
            // Higher rating should be seeded earlier (top)
            return participants.OrderByDescending(p => p.Rating ?? int.MinValue).ToList();
        }
    }
}
