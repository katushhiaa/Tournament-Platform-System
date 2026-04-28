using System;
using System.Collections.Generic;

namespace TournamentPlatformSystem.Application.DTOs.Auth
{
    public class UserStatsDto
    {
        public DateOnly? DateOfBirth { get; set; }
        public List<string>? Phones { get; set; }
    }
}
