using System.ComponentModel.DataAnnotations;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public bool RememberMe { get; set; } = false;
    }
}
