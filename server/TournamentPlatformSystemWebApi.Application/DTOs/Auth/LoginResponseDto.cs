using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth;

public class LoginResponseDto
{
    public TokensResponseDto? Tokens { get; set; }
    public UserDto? User { get; set; }
}
