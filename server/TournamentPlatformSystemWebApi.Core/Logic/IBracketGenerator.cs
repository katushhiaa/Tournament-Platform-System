using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public interface IBracketGenerator
    {
        /// <summary>
        /// Generate initial matches for a tournament and return a list of Match entities representing the first-round bracket.
        /// Implementations should be deterministic given the same seeded participant order and tournament id.
        /// </summary>
        IReadOnlyList<Match> Generate(Guid tournamentId, IReadOnlyList<BracketParticipant> participants, ISeedingStrategy seedingStrategy);
    }
}
