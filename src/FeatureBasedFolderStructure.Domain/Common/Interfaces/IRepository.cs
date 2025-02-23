namespace FeatureBasedFolderStructure.Domain.Common.Interfaces;

public interface IRepository<TEntity, TKey> 
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    IQueryable<TEntity> AsQueryable();
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool asNoTracking = false);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity,CancellationToken cancellationToken);
}