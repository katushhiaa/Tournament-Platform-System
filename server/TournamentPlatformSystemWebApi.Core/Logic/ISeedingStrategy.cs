using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public interface ISeedingStrategy
    {
        IReadOnlyList<BracketParticipant> Seed(IReadOnlyList<BracketParticipant> participants);
    }
}
