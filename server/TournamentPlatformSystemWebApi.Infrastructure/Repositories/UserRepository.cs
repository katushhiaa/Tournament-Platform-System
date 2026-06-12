using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Core.Entities;
using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Security;

namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, UserModel>, IUserRepository
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(TournamentdbContext context, IMapper mapper, IPasswordHasher passwordHasher) : base(context, mapper)
        {
            _passwordHasher = passwordHasher;
        }

        private bool ShouldUseTransaction()
        {
            var provider = _context.Database.ProviderName ?? string.Empty;
            return _context.Database.CurrentTransaction == null && !provider.Contains("InMemory", StringComparison.OrdinalIgnoreCase);
        }

        public async override Task<Guid> CreateAsync(User entity)
        {
            if (entity.Password == null)
            {
                return Guid.Empty;
            }

            var useTransaction = ShouldUseTransaction();
            await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
            try
            {
                var paswordHash = _passwordHasher.HashPassword(entity.Password);

                var dbModel = _mapper.Map<UserModel>(entity);
                dbModel.Id = Guid.NewGuid();

                dbModel.PasswordHash = paswordHash;
                dbModel.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                // if account state not provided, try to resolve "active"
                if (dbModel.AccountStateId == Guid.Empty)
                {
                    var activeState = await _context.Set<AccountStateModel>().FirstOrDefaultAsync(s => s.IsActive ?? false);
                    if (activeState != null)
                    {
                        dbModel.AccountStateId = activeState.Id;
                    }
                }

                await _context.Set<UserModel>().AddAsync(dbModel);
                await _context.SaveChangesAsync();

                // create user detail if provided
                if (entity.UserDetail != null)
                {
                    var userDetail = new UserDetailModel
                    {
                        Id = Guid.NewGuid(),
                        UserId = dbModel.Id,
                        Email = entity.UserDetail.Email,
                        DateOfBirth = entity.UserDetail.DateOfBirth,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                    };

                    await _context.Set<UserDetailModel>().AddAsync(userDetail);

                    // create phones
                    if (entity.UserDetail.Phones != null)
                    {
                        foreach (var phone in entity.UserDetail.Phones)
                        {
                            var userPhone = new UserPhoneModel
                            {
                                Id = Guid.NewGuid(),
                                UserId = dbModel.Id,
                                PhoneNumber = phone,
                                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                            };

                            await _context.Set<UserPhoneModel>().AddAsync(userPhone);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                if (useTransaction && txn != null)
                {
                    await txn.CommitAsync();
                }

                return dbModel.Id;
            }
            catch
            {
                if (useTransaction && txn != null)
                {
                    await txn.RollbackAsync();
                }

                throw;
            }
        }

        public async Task<User> GetUserWithDetails(Guid id)
        {
            var dbModel = await _context.Set<UserModel>()
            .Include(x => x.AccountState)
            .Include(x => x.UserDetail)
            .Include(x => x.UserPhones)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<User>(dbModel);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Set<UserModel>().AnyAsync(x => x.Id == userId);
        }

        public async Task<bool?> GetPreferencesSetupCompletedAsync(Guid userId)
        {
            var detail = await _context.Set<UserDetailModel>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return detail?.PreferencesSetupCompleted;
        }

        public async Task<bool> SetPreferencesSetupCompletedAsync(Guid userId, bool value)
        {
            var detail = await _context.Set<UserDetailModel>()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (detail == null)
            {
                return false;
            }

            detail.PreferencesSetupCompleted = value;
            _context.Set<UserDetailModel>().Update(detail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SetUserThemePreferencesAsync(Guid userId, IReadOnlyCollection<Guid> themeIds)
        {
            var useTransaction = ShouldUseTransaction();
            await using var txn = useTransaction ? await _context.Database.BeginTransactionAsync() : null;
            try
            {
                var existing = await _context.Set<UserTournamentThemePreferenceModel>()
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (existing.Any())
                {
                    _context.Set<UserTournamentThemePreferenceModel>().RemoveRange(existing);
                }

                if (themeIds != null && themeIds.Count > 0)
                {
                    var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    var items = themeIds.Distinct().Select(themeId => new UserTournamentThemePreferenceModel
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ThemeId = themeId,
                        CreatedAt = now
                    });

                    await _context.Set<UserTournamentThemePreferenceModel>().AddRangeAsync(items);
                }

                await _context.SaveChangesAsync();

                await SetPreferencesSetupCompletedAsync(userId, true);

                if (useTransaction && txn != null)
                {
                    await txn.CommitAsync();
                }
            }
            catch
            {
                if (useTransaction && txn != null)
                {
                    await txn.RollbackAsync();
                }

                throw;
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Set<UserDetailModel>().AnyAsync(x => x.Email == email);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var dbModel = await _context.Set<UserModel>()
            .Include(x => x.AccountState)
            .Include(x => x.UserDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserDetail != null && x.UserDetail.Email == email);

            return _mapper.Map<User>(dbModel);
        }

        public async Task<string?> GetPasswordHashByEmailAsync(string email)
        {
            var hash = await _context.Set<UserModel>()
                .AsNoTracking()
                .Where(x => x.UserDetail != null && x.UserDetail.Email == email)
                .Select(x => x.PasswordHash)
                .FirstOrDefaultAsync();

            return hash;
        }

        public async Task SetRefreshTokenForUser(Guid userId, string token, string jwtId, DateTime expiresAt)
        {

            await RevokeUserTokens(userId);

            var refreshModel = new RefreshTokenModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                JwtId = jwtId,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Unspecified),
                IsUsed = false,
                IsRevoked = false
            };

            await _context.Set<RefreshTokenModel>().AddAsync(refreshModel);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateRefreshTokenForUser(Guid userId, string token, string jwtId)
        {

            var refresh = await _context.Set<RefreshTokenModel>()
                .FirstOrDefaultAsync(x => x.Token == token && x.UserId == userId);

            if (refresh == null)
            {
                return false;
            }

            if (refresh.UserId != userId || refresh.JwtId != jwtId)
            {
                return false;
            }

            if (refresh.IsUsed || refresh.IsRevoked)
            {
                return false;
            }

            if (refresh.ExpiresAt <= DateTime.UtcNow)
            {
                return false;
            }

            refresh.IsUsed = true;
            _context.Set<RefreshTokenModel>().Update(refresh);
            await _context.SaveChangesAsync();

            return true;
        }

        // Revoke any existing (non-revoked) refresh tokens for user
        public async Task RevokeUserTokens(Guid userId)
        {
            var existing = await _context.Set<RefreshTokenModel>()
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync();

            if (existing.Any())
            {
                foreach (var t in existing)
                {
                    t.IsRevoked = true;
                }

                _context.Set<RefreshTokenModel>().UpdateRange(existing);
            }
        }

        public async Task<IReadOnlyList<UserSearchItemResponce>> SearchUsersAsync(string query, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await _context.Set<UserModel>()
                .AsNoTracking()
                .Where(u => u.IsOrganizer != true)
                .OrderBy(u => u.FullName)
                .Select(u => new UserSearchItemResponce { Id = u.Id, FullName = u.FullName })
                .Take(limit)
                .ToListAsync();
            }

            var q = query.Trim().ToLowerInvariant();

            var results = await _context.Set<UserModel>()
                .AsNoTracking()
                .Include(u => u.UserDetail)
                .Where(u => u.IsOrganizer != true && (EF.Functions.Like(u.FullName.ToLower(), $"%{q}%")
                            || (u.UserDetail != null && EF.Functions.Like(u.UserDetail.Email.ToLower(), $"%{q}%"))))
                .OrderBy(u => u.FullName)
                .Select(u => new UserSearchItemResponce { Id = u.Id, FullName = u.FullName })
                .Take(limit)
                .ToListAsync();

            return results;
        }
    }
}