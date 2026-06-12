using TournamentPlatformSystemWebApi.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IAuthenticationService
{
    Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<TokensResponseDto> RefreshAsync(string refreshToken, string accessToken);
}
