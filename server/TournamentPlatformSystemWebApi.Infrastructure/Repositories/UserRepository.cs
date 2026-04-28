using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
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

        public async override Task<Guid> CreateAsync(User entity)
        {
            if (entity.Password == null)
            {
                return Guid.Empty;
            }

            var provider = _context.Database.ProviderName ?? string.Empty;
            var useTransaction = !provider.Contains("InMemory", StringComparison.OrdinalIgnoreCase);

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
                    var activeState = await _context.Set<AccountStateModel>().FirstOrDefaultAsync(s => s.Name == "active");
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
            .Include(x => x.UserDetail)
            .Include(x => x.UserPhones)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<User>(dbModel);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Set<UserDetailModel>().AnyAsync(x => x.Email == email);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var dbModel = await _context.Set<UserModel>()
            .Include(x => x.UserDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserDetail != null && x.UserDetail.Email == email);

            return _mapper.Map<User>(dbModel);
        }
    }
}