using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace FeatureBasedFolderStructure.Domain.Common.Interfaces;

public interface IRepository<TEntity, TKey> 
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    IQueryable<TEntity> AsQueryable();
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool enableTracking = false);
    Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0, int size = 10,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity,CancellationToken cancellationToken);
}