using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Models;
using TournamentPlatformSystemWebApi.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [SwaggerTag("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Реєстрація нового користувача", Description = "Реєструє нового користувача. Роль: Guest.")]
        [SwaggerResponse(201, Type = typeof(RegisterUserResponse), Description = "Повертає токен та дані користувача")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Email вже існує")]
        [SwaggerRequestExample(typeof(RegisterUserRequest), typeof(Swagger.Examples.RegisterUserRequestExample))]
        [SwaggerResponseExample(201, typeof(Swagger.Examples.RegisterUserResponseExample))]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest dto)
        {
            try
            {
                var result = await _authenticationService.RegisterAsync(dto);
                return Created(string.Empty, result);
            }
            catch (ValidationException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 400,
                        Type = "ValidationError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return BadRequest(err);
            }
            catch (DuplicateEmailException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 409,
                        Type = "Conflict",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return Conflict(err);
            }
            catch (Exception)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 500,
                        Type = "InternalServerError",
                        Message = "Internal server error",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return StatusCode(500, err);
            }
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Авторизація користувача", Description = "Авторизує користувача за email/password. Роль: Guest.")]
        [SwaggerResponse(200, Type = typeof(LoginResponseDto), Description = "Успішна авторизація")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Помилка валідації")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Невірний логін/пароль")]
        [SwaggerRequestExample(typeof(LoginRequestDto), typeof(Swagger.Examples.LoginRequestExample))]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.LoginResponseExample))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var tokens = await _authenticationService.LoginAsync(dto);
                return Ok(tokens);
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TooManyLoginAttemptsException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status429TooManyRequests,
                        Type = "TooManyRequests",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return StatusCode(StatusCodes.Status429TooManyRequests, err);
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.InvalidCredentialsException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Type = "Unauthorized",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return Unauthorized(err);
            }
            catch (ValidationException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 400,
                        Type = "ValidationError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return BadRequest(err);
            }
            catch (ForbiddenException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 500,
                        Type = "InternalServerError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return StatusCode(500, err);
            }
        }

        /// <summary>
        /// Refreshes the access token using a refresh token stored in an HttpOnly cookie.
        /// </summary>
        /// <remarks>
        /// WARNING! This endpoint cannot be tested through swagger because it will not be possible to pass cookies. Client must send the refresh token in a cookie named "refresh_token" (HttpOnly, Secure recommended).
        /// The endpoint validates the refresh, access tokens and, on success, returns a new access token and a new refresh token in the response body.
        /// </remarks>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Refresh access token", Description = "WARNING! This endpoint cannot be tested through swagger because it will not be possible to pass cookies. Refreshes the access token using the refresh token stored in an HttpOnly cookie named 'refresh_token'.")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TokensResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.TokenResponseExample))]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var refreshToken = Request.Cookies["refresh_token"];
                var authHeader = Request.Headers["Authorization"].ToString();
                var tokens = await _authenticationService.RefreshAsync(refreshToken, authHeader);
                return Ok(tokens);
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.InvalidCredentialsException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Type = "Unauthorized",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return Unauthorized(err);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 500,
                        Type = "InternalServerError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return StatusCode(500, err);
            }
        }

        [Authorize]
        [HttpGet("/api/v1/users/me")]
        [SwaggerOperation(Summary = "Отримання даних поточного користувача", Description = "Повертає профіль поточного користувача. Роль: Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(UserDto), Description = "Дані користувача")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Токен відсутній або недійсний")]
        public IActionResult Me()
        {
            var user = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "player@example.com",
                FullName = "Sample Player",
                Role = "Player"
            };
            return Ok(user);
        }
    }
}

