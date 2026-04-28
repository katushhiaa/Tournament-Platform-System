using System;
using System.Collections.Generic;
using System.Linq;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public sealed class SingleEliminationBracketGenerator : IBracketGenerator
    {
        /// <summary>
        /// Generate all-rounds matches for a single-elimination bracket.
        /// Uses the provided seeding strategy to order participants before pairing.
        /// </summary>
        public IReadOnlyList<Match> Generate(Guid tournamentId, IReadOnlyList<BracketParticipant> participants, ISeedingStrategy seedingStrategy)
        {
            if (participants == null) throw new ArgumentNullException(nameof(participants));
            if (seedingStrategy == null) throw new ArgumentNullException(nameof(seedingStrategy));

            var seeded = seedingStrategy.Seed(participants).ToList();

            // Compute next power of two for bracket size
            int n = seeded.Count;
            int bracketSize = 1;
            while (bracketSize < n) bracketSize <<= 1;

            // Build ordered list for first round using classical seeding placement (distribute top seeds across bracket)
            BracketParticipant?[] ordered = new BracketParticipant?[bracketSize];
            var seedOrder = GetSeedOrder(bracketSize); // gives seed numbers in slot order
            for (int slot = 0; slot < bracketSize; slot++)
            {
                int seedNumber = seedOrder[slot];
                int participantIndex = seedNumber - 1; // seeded list is 0-based by seed
                if (participantIndex < n)
                    ordered[slot] = seeded[participantIndex];
                else
                    ordered[slot] = null;
            }

            var matches = new List<Match>();
            int orderNumber = 1;

            BuildRounds(tournamentId, ordered.ToList(), 1, ref orderNumber, matches);

            return matches;
        }

        private void BuildRounds(Guid tournamentId, List<BracketParticipant?> participants, int level, ref int orderNumber, List<Match> outMatches)
        {
            if (participants == null) throw new ArgumentNullException(nameof(participants));

            int slotCount = participants.Count;
            if (slotCount < 2) return; // no matches to create

            var nextRoundParticipants = new List<BracketParticipant?>(slotCount / 2);

            for (int i = 0; i < slotCount; i += 2)
            {
                var a = participants[i];
                var b = participants[i + 1];

                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    TeamAId = a?.Id ?? Guid.Empty,
                    TeamBId = b?.Id == null || b?.Id == Guid.Empty ? null : b?.Id,
                    Level = level,
                    OrderNumber = orderNumber++,
                    StartDate = null,
                    IsValid = true,
                    TeamAName = a?.Name,
                    TeamBName = b?.Name
                };

                // If opponent is missing (bye), auto-advance TeamA
                if (b == null)
                {
                    match.TeamBId = null;
                    match.WinnerId = match.TeamAId;
                    // Represent next-round participant as the actual advancing team
                    nextRoundParticipants.Add(new BracketParticipant { Id = match.TeamAId, Name = match.TeamAName ?? string.Empty });
                }
                else
                {
                    // Placeholder for winner; Id will be Guid.Empty until match is played
                    nextRoundParticipants.Add(new BracketParticipant { Id = Guid.Empty, Name = string.Empty });
                }

                outMatches.Add(match);
            }

            // Recurse for next round
            BuildRounds(tournamentId, nextRoundParticipants, level + 1, ref orderNumber, outMatches);
        }

        private static int[] GetSeedOrder(int bracketSize)
        {
            if (bracketSize == 1) return new[] { 1 };
            if (bracketSize % 2 != 0) throw new ArgumentException("bracketSize must be a power of two", nameof(bracketSize));

            var half = bracketSize / 2;
            var prev = GetSeedOrder(half);
            var result = new int[bracketSize];
            for (int i = 0; i < half; i++)
            {
                result[i * 2] = prev[i];
                result[i * 2 + 1] = bracketSize + 1 - prev[i];
            }

            return result;
        }
    }
}
