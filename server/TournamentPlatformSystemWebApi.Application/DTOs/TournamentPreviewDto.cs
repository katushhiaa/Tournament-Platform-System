using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TournamentPreviewDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string? BackgroundImg { get; set; }
        public string? SportName { get; set; }
        public DateTime StartDate { get; set; }
        public int ParticipantsCount { get; set; }
        public int MaxParticipants { get; set; }
    }
}
