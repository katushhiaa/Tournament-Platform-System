using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Helpers;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories;

public class BaseRepository<TEntity, TDBEntity> : IRepository<TEntity, Guid> where TEntity : BaseEntity where TDBEntity : BaseDbEntity
{
    protected readonly TournamentdbContext _context;
    protected readonly IMapper _mapper;

    public BaseRepository(TournamentdbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public virtual async Task<Guid> CreateAsync(TEntity entity)
    {
        var dbModel = _mapper.Map<TDBEntity>(entity);

        await _context.Set<TDBEntity>().AddAsync(dbModel);
        await _context.SaveChangesAsync();

        return dbModel.Id;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var dbModel = await _context.Set<TDBEntity>().FindAsync(id);

        if (dbModel == null)
            return false;

        _context.Set<TDBEntity>().Remove(dbModel);
        await _context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await _context.Set<TDBEntity>()
            .AsNoTracking()
            .ProjectTo<TEntity>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var dbModel = await _context.Set<TDBEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<TEntity>(dbModel);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var existingDbModel = await _context.Set<TDBEntity>()
            .FindAsync(entity.Id);

        if (existingDbModel == null)
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");

        _mapper.Map(entity, existingDbModel);

        _context.Set<TDBEntity>().Update(existingDbModel);
        await _context.SaveChangesAsync();

        return _mapper.Map<TEntity>(existingDbModel);
    }
}
