using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TournamentDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Conditions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationCloseDate { get; set; }
        public Guid? Sport { get; set; }
        public int MaxParticipants { get; set; }
        public string? Status { get; set; }
        public Guid OrganizerId { get; set; }
    }
}
