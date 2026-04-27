using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class RegisterUserResponse
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
    }
}
