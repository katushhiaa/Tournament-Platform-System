using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories;

public class TournamentRepository : BaseRepository<Tournament, TournamentModel>, ITournamentRepository
{
    public TournamentRepository(TournamentdbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public override async Task<Tournament?> GetByIdAsync(Guid id)
    {
        var dbModel = await _context.Set<TournamentModel>()
            .Include(x => x.Organizer)
            .Include(x => x.Theme)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<Tournament>(dbModel);
    }

    public override async Task<ICollection<Tournament>> GetAllAsync()
    {
        return await _context.Set<TournamentModel>()
            .Include(x => x.Theme)
            .AsNoTracking()
            .ProjectTo<Tournament>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> IsTitleUniqueAsync(string title, Guid organizerId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return true;

        var normalized = title.Trim().ToLowerInvariant();

        var exists = await _context.Set<TournamentModel>()
            .AsNoTracking()
            .AnyAsync(t => t.OrganizerId == organizerId
                           && t.Name.ToLower() == normalized);

        return !exists;
    }

}

