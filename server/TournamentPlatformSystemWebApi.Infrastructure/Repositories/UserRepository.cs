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
            var paswordHash = _passwordHasher.HashPassword(entity.Password);

            var dbModel = _mapper.Map<UserModel>(entity);

            dbModel.PasswordHash = paswordHash;

            await _context.Set<UserModel>().AddAsync(dbModel);
            await _context.SaveChangesAsync();

            return dbModel.Id;
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
    }
}