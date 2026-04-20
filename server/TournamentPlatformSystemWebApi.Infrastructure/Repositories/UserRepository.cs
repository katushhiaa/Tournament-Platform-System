// using System;
// using System.Data.Common;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using TournamentPlatformSystemWebApi.Infrastructure.Entities;
// using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
// using TournamentPlatformSystemWebApi.Application.Interfaces;
// using TournamentPlatformSystemWebApi.Infrastructure.Context;

// namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories
// {
//     public class UserRepository : IUserRepository
//     {
//         private readonly TournamentdbContext _context;

//         public UserRepository(TournamentdbContext context)
//         {
//             _context = context;
//         }

//         public async Task CreateAsync(UserDto user)
//         {
//             // The InMemory provider does not support transactions and will emit a warning
//             // that can surface as an exception in tests. Avoid beginning a transaction
//             // when running against the in-memory provider.
//             var isInMemory = _context.Database.ProviderName?.Contains("InMemory", StringComparison.OrdinalIgnoreCase) == true;

//             if (isInMemory)
//             {
//                 var accountState = await _context.AccountStates.FirstOrDefaultAsync(s => s.Name == "active");
//                 if (accountState == null)
//                     throw new InvalidOperationException("Default account state 'active' not found");

//                 var userModel = new UserModel
//                 {
//                     Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id,
//                     FullName = user.Name ?? string.Empty,
//                     PasswordHash = string.Empty,
//                     IsOrganizer = false,
//                     AccountStateId = accountState.Id
//                 };

//                 _context.Users.Add(userModel);
//                 await _context.SaveChangesAsync();

//                 if (!string.IsNullOrWhiteSpace(user.Email))
//                 {
//                     var userDetail = new UserDetailModel
//                     {
//                         UserId = userModel.Id,
//                         Email = user.Email,
//                         DateOfBirth = DateOnly.MinValue
//                     };

//                     _context.UserDetails.Add(userDetail);
//                     await _context.SaveChangesAsync();
//                 }

//                 return;
//             }

//             await using var transaction = await _context.Database.BeginTransactionAsync();
//             try
//             {
//                 var accountState = await _context.AccountStates.FirstOrDefaultAsync(s => s.Name == "active");
//                 if (accountState == null)
//                     throw new InvalidOperationException("Default account state 'active' not found");

//                 var userModel = new UserModel
//                 {
//                     Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id,
//                     FullName = user.Name ?? string.Empty,
//                     PasswordHash = string.Empty,
//                     IsOrganizer = false,
//                     AccountStateId = accountState.Id
//                 };

//                 _context.Users.Add(userModel);
//                 await _context.SaveChangesAsync();

//                 if (!string.IsNullOrWhiteSpace(user.Email))
//                 {
//                     var userDetail = new UserDetailModel
//                     {
//                         UserId = userModel.Id,
//                         Email = user.Email,
//                         DateOfBirth = DateOnly.MinValue
//                     };

//                     _context.UserDetails.Add(userDetail);
//                     await _context.SaveChangesAsync();
//                 }

//                 await transaction.CommitAsync();
//             }
//             catch
//             {
//                 await transaction.RollbackAsync();
//                 throw;
//             }
//         }

//         public async Task<bool> ExistsByEmailAsync(string email)
//         {
//             if (string.IsNullOrWhiteSpace(email))
//                 return false;

//             return await _context.UserDetails.CountAsync(d => d.Email == email)
//                 .ContinueWith(t => t.Result > 0);
//         }

//         public async Task<UserDto?> GetByEmailAsync(string email)
//         {
//             if (string.IsNullOrWhiteSpace(email))
//                 return null;

//             var userDetail = await _context.UserDetails
//                 .Include(d => d.User)
//                 .FirstOrDefaultAsync(d => d.Email == email);

//             if (userDetail == null)
//                 return null;

//             var user = userDetail.User;

//             return new UserDto
//             {
//                 Id = user.Id,
//                 Email = userDetail.Email,
//                 Name = user.FullName,
//                 Role = user.IsOrganizer == true ? "Organizer" : "Player",
//                 Stats = null
//             };
//         }


//         public async Task<UserDto?> GetByIdAsync(Guid id)
//         {
//             var user = await _context.Users
//                 .Include(u => u.UserDetail)
//                 .Include(u => u.UserPhones)
//                 .FirstOrDefaultAsync(u => u.Id == id);

//             if (user == null)
//                 return null;

//             return new UserDto
//             {
//                 Id = user.Id,
//                 Email = user.UserDetail?.Email,
//                 Name = user.FullName,
//                 Role = user.IsOrganizer == true ? "Organizer" : "Player",
//                 Stats = new TournamentPlatformSystemWebApi.Application.DTOs.Auth.UserStatsDto
//                 {
//                     DateOfBirth = user.UserDetail?.DateOfBirth,
//                     Phones = user.UserPhones != null ? user.UserPhones.Select(p => p.PhoneNumber).ToList() : null
//                 }
//             };
//         }
//     }
// }