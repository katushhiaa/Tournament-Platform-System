using System;
using System.ComponentModel.DataAnnotations;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TournamentCreateDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(2000)]
        public string? Conditions { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime RegistrationCloseDate { get; set; }

        [Required]
        public Guid? Sport { get; set; }

        [Range(2, 1024)]
        public int MaxParticipants { get; set; } = 2;


    }
}
