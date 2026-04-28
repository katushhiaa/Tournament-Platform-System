using System;
using TournamentPlatformSystemWebApi.Common.Helpers;
namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IRepository<TEntity, TKey> where TEntity : BaseEntity
{
    Task<ICollection<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TKey> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TKey id);

}
