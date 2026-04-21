using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Exceptions;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Security;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly TournamentdbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUserRepository userRepository, TournamentdbContext db, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest request)
    {
        if (request.Email == null || request.Password == null || request.FullName == null || request.PhoneNumber == null || request.DateOfBirth == null || request.Role == null)
        {
            throw new ValidationException("Missing required fields");
        }

        // check duplicate email
        if (await _userRepository.ExistsByEmailAsync(request.Email))
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

        // Begin transaction and create DB models
        await using var txn = await _db.Database.BeginTransactionAsync();
        try
        {
            // resolve active account state id if exists
            var activeState = await _db.AccountStates.FirstOrDefaultAsync(s => s.Name == "active");
            var accountStateId = activeState?.Id ?? Guid.Empty;

            var userModel = new UserModel
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                IsOrganizer = string.Equals(request.Role, "Organizer", StringComparison.OrdinalIgnoreCase),
                AccountStateId = accountStateId,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            // hash password and set
            userModel.PasswordHash = _passwordHasher.HashPassword(request.Password);

            await _db.Users.AddAsync(userModel);

            var userDetail = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                UserId = userModel.Id,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth.Value,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            await _db.UserDetails.AddAsync(userDetail);

            var userPhone = new UserPhoneModel
            {
                Id = Guid.NewGuid(),
                UserId = userModel.Id,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            await _db.UserPhones.AddAsync(userPhone);

            await _db.SaveChangesAsync();
            await txn.CommitAsync();

            // generate jwt
            var token = GenerateJwtToken(userModel.Id, request.Email, request.Role);

            return new RegisterUserResponse
            {
                UserId = userModel.Id,
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                Token = token
            };
        }
        catch
        {
            await txn.RollbackAsync();
            throw;
        }
    }

    private string GenerateJwtToken(Guid userId, string email, string role)
    {
        var key = _configuration["Jwt:Key"] ?? "default-development-key-change-in-production";
        var issuer = _configuration["Jwt:Issuer"] ?? "tournament-api";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
