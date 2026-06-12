using System;
using System.Collections.Generic;
using System.Linq;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Core.Logic
{
    public static class MatchStatusHelper
    {
        /// <summary>
        /// Assigns Status and IsBye flags to a collection of matches.
        /// Rules:
        /// - If TeamAId or TeamBId is Guid.Empty (placeholder) => Status = "pending"
        /// - If TeamBId == null => IsBye = true, Status = "bye" (Winner should already be set)
        /// - If WinnerId != null => Status = "completed"
        /// - If StartDate != null and no winner => Status = "in_progress"
        /// - Otherwise => Status = "scheduled"
        ///
        /// This mutates the provided match objects.
        /// </summary>
        public static void AssignStatuses(IEnumerable<Match> matches)
        {
            if (matches == null) return;

            foreach (var m in matches)
            {
                m.IsBye = false;

                // placeholders: unknown opponent
                var teamAEmpty = m.TeamAId == Guid.Empty;
                var teamBEmpty = m.TeamBId == Guid.Empty;

                if (teamAEmpty || teamBEmpty)
                {
                    m.IsBye = false;
                    m.Status = "pending";
                    continue;
                }

                if (m.TeamBId == null)
                {
                    m.IsBye = true;
                    m.Status = "bye";
                    // ensure WinnerId is set (generator usually sets it)
                    if (m.WinnerId == null && m.TeamAId != Guid.Empty)
                        m.WinnerId = m.TeamAId;
                    continue;
                }

                if (m.WinnerId != null && m.WinnerId != Guid.Empty)
                {
                    m.IsBye = false;
                    m.Status = "completed";
                    continue;
                }

                if (m.StartDate != null)
                {
                    m.IsBye = false;
                    m.Status = "in_progress";
                    continue;
                }

                m.IsBye = false;
                m.Status = "scheduled";
            }
        }
    }
}
