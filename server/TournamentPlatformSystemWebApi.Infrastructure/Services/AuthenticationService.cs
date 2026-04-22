using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
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
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(IUserRepository userRepository, TournamentdbContext db, IPasswordHasher passwordHasher, TournamentPlatformSystemWebApi.Infrastructure.Security.IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
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

        var userEntity = new TournamentPlatformSystemWebApi.Core.Entities.User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            IsOrganizer = string.Equals(request.Role, "Organizer", StringComparison.OrdinalIgnoreCase),
            Password = request.Password,
            UserDetail = new TournamentPlatformSystemWebApi.Core.Entities.UserDetail
            {
                Email = request.Email,
                DateOfBirth = request.DateOfBirth.Value,
                Phones = new List<string> { request.PhoneNumber }
            }
        };

        var createdUserId = await _userRepository.CreateAsync(userEntity);

        // generate jwt
        var token = _jwtTokenService.GenerateToken(createdUserId, request.Email, request.Role);

        return new RegisterUserResponse
        {
            UserId = createdUserId,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            Token = token
        };
    }


}
