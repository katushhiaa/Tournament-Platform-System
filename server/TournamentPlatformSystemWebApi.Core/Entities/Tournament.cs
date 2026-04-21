using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int MaxTeams { get; set; }
        public string? BackgroundImg { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public string? Conditions { get; set; }
        public TournamentStatus Status { get; set; }
        public Guid? OrganizerId { get; set; }
        public string? OrganizerName { get; set; }
        public string? ThemeName { get; set; }
    }
}
