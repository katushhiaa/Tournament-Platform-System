using System;
using System.ComponentModel.DataAnnotations;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class RegisterUserRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email format is invalid")]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(128, ErrorMessage = "Password must not exceed 128 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,128}$", ErrorMessage = "Password must be at least 8 characters and not exceed 128 characters, contain an uppercase letter and a digit")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(255, ErrorMessage = "Full name must not exceed 255 characters")]
        [RegularExpression(
            @"^[A-Za-zА-Яа-яІіЇїЄєҐґ''-]+(?:\s+[A-Za-zА-Яа-яІіЇїЄєҐґ''-]+){1,2}$",
            ErrorMessage = "Full name must contain 2-3 words with only letters, apostrophes and hyphens"
        )]
        public string? FullName { get; set; }

        [Required]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone must be in format +380XXXXXXXXX")]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateOnly? DateOfBirth { get; set; }

        [Required]
        [RegularExpression("^(organizer|player)$", ErrorMessage = "Role must be 'organizer' or 'player'")]
        public string? Role { get; set; }
    }
}
