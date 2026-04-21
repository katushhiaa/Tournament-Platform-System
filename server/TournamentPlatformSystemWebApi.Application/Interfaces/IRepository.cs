using System;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IRepository<TEntity, TKey>
{
    Task<ICollection<TEntity>> GetAsync();
    Task<TKey> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TKey id);

}
