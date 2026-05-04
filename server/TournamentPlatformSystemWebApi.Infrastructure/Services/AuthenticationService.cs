using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Exceptions;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Security;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMemoryCache _cache;

    public AuthenticationService(IUserRepository userRepository, TournamentdbContext db, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, IMemoryCache? cache = null)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _cache = cache ?? new MemoryCache(new MemoryCacheOptions());
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        if (request.Email == null || request.Password == null || request.FullName == null || request.PhoneNumber == null || request.DateOfBirth == null || request.Role == null)
        {
            throw new ValidationException("Missing required fields");
        }
        // normalize email (case-insensitive handling)
        var email = request.Email.ToLowerInvariant();

        // check duplicate email
        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new DuplicateEmailException("Email is already in use");
        }

        // password validation
        var pwdRegex = new Regex("^(?=.*[A-Z])(?=.*\\d).{8,}$");
        if (!pwdRegex.IsMatch(request.Password))
        {
            throw new ValidationException("Password must be at least 8 characters, contain an uppercase letter and a digit");
        }

        // date of birth validation (age >= 13)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dob = request.DateOfBirth.Value;
        var age = today.Year - dob.Year - (today < dob.AddYears(today.Year - dob.Year) ? 1 : 0);
        if (age < 13)
        {
            throw new ValidationException("User must be at least 13 years old");
        }

        // phone validation
        var phoneRegex = new Regex("^\\+380\\d{9}$");
        if (!phoneRegex.IsMatch(request.PhoneNumber))
        {
            throw new ValidationException("Phone number must match +380XXXXXXXXX");
        }

        var userEntity = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            IsOrganizer = string.Equals(request.Role, "Organizer", StringComparison.OrdinalIgnoreCase),
            Password = request.Password,
            UserDetail = new UserDetail
            {
                Email = email,
                DateOfBirth = request.DateOfBirth.Value,
                Phones = new List<string> { request.PhoneNumber }
            }
        };

        var createdUserId = await _userRepository.CreateAsync(userEntity);

        var isOrganizer = string.Equals(request.Role, "organizer", StringComparison.OrdinalIgnoreCase);
        var token = _jwtTokenService.GenerateToken(createdUserId, email, request.Role, isOrganizer);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new RegisterUserResponse
        {
            UserId = createdUserId,
            Email = email,
            FullName = request.FullName,
            Role = request.Role,
            Tokens = new TokensResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (request.Email == null || request.Password == null)
        {
            throw new ValidationException("Missing email or password");
        }
        var email = request.Email.ToLowerInvariant();
        var emailKey = email;
        var blockedKey = $"login_blocked:{emailKey}";
        var failedKey = $"login_failed:{emailKey}";

        if (_cache.TryGetValue(blockedKey, out _))
        {
            throw new TooManyLoginAttemptsException("Too many failed login attempts. Try again later.");
        }
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            LoginFailedAttempt(failedKey, blockedKey);
            throw new InvalidCredentialsException("Invalid email or password");
        }
        var storedHash = await _userRepository.GetPasswordHashByEmailAsync(email) ?? string.Empty;
        var verified = _passwordHasher.VerifyPassword(request.Password, storedHash);
        if (!verified)
        {
            LoginFailedAttempt(failedKey, blockedKey);
            throw new InvalidCredentialsException("Invalid email or password");
        }

        _cache.Remove(failedKey);
        _cache.Remove(blockedKey);

        if (!user.IsActive)
        {
            throw new ForbiddenException("User account is inactive. Please contact support.");
        }

        var isOrganizer = user.IsOrganizer ?? false;
        var access = _jwtTokenService.GenerateToken(user.Id, user.UserDetail?.Email ?? string.Empty, isOrganizer ? "organizer" : "player", isOrganizer);

        var refresh = request.RememberMe ? _jwtTokenService.GenerateRefreshToken() : null;

        var response = new LoginResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Email = user.UserDetail?.Email,
                Role = isOrganizer ? "organizer" : "player",
                FullName = user.FullName
            },
            Tokens = new TokensResponseDto { AccessToken = access, RefreshToken = refresh }
        };
        return response;
    }

    private void LoginFailedAttempt(string failedKey, string blockedKey)
    {
        const int maxAttempts = 5;
        var attempts = 0;
        if (_cache.TryGetValue(failedKey, out var obj) && obj is int current)
        {
            attempts = current;
        }
        attempts++;

        if (attempts >= maxAttempts)
        {
            _cache.Set(blockedKey, true, TimeSpan.FromMinutes(5));
            _cache.Remove(failedKey);
        }
        else
        {
            _cache.Set(failedKey, attempts, TimeSpan.FromMinutes(5));
        }
    }
}
