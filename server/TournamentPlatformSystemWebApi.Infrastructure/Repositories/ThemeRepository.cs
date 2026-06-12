using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories;

public class ThemeRepository : BaseRepository<TournamentTheme, TournamentThemeModel>, IThemeRepository
{
    public ThemeRepository(TournamentdbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<bool> IsSportWithId(Guid id)
    {
        return await _context.Set<TournamentThemeModel>().AnyAsync(x => x.Id == id);
    }

    public async Task<bool> AreThemeIdsValidAsync(IReadOnlyCollection<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return true;
        }

        var distinctIds = ids.Distinct().ToList();
        var count = await _context.Set<TournamentThemeModel>()
            .CountAsync(x => distinctIds.Contains(x.Id));

        return count == distinctIds.Count;
    }
}
