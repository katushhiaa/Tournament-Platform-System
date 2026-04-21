using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class Match : BaseEntity
    {
        public Guid TournamentId { get; set; }
        public Guid TeamAId { get; set; }
        public Guid? TeamBId { get; set; }
        public Guid? WinnerId { get; set; }
        public int Level { get; set; }
        public int OrderNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public int? TeamAScore { get; set; }
        public int? TeamBScore { get; set; }
        public bool? IsValid { get; set; }

        // Business model: keep simple reference fields only
        public string? TeamAName { get; set; }
        public string? TeamBName { get; set; }
    }
}
