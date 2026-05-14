using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TournamentDetailsDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Conditions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationCloseDate { get; set; }
        public Guid? SportId { get; set; }
        public string? SportName { get; set; }
        public int MaxParticipants { get; set; }
        public string? Status { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string? BackgroundImg { get; set; }
        public int ParticipantsCount { get; set; }

        public IReadOnlyList<MatchesRoundDto> Matches { get; set; } = new List<MatchesRoundDto>();

    }
}
