using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public object? Stats { get; set; }
    }
}
