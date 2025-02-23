using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<TEntity, TKey>(DbContext context) : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual IQueryable<TEntity> AsQueryable() => _dbSet;

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool asNoTracking = false) =>
        asNoTracking
            ? await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
            : await _dbSet.FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken) => await _dbSet.ToListAsync(cancellationToken);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var hasIsDeleted = typeof(TEntity).GetProperty("IsDeleted") != null;
    
        if (hasIsDeleted)
        {
            var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
            isDeletedProperty!.SetValue(entity, true);
            context.Entry(entity).State = EntityState.Modified;
        }
        else
            _dbSet.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}