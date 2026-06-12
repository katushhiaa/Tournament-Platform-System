using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }
}
