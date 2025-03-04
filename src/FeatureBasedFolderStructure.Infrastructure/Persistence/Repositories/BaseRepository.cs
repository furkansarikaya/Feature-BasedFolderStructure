using System.Linq.Expressions;
using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<TEntity, TKey>(DbContext context) : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual IQueryable<TEntity> AsQueryable() => _dbSet;

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool enableTracking = false) =>
        enableTracking
            ? await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
            : await _dbSet.FindAsync([id], cancellationToken);

    public async Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0, int size = 10,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = AsQueryable();
        if (enableTracking) query = query.AsTracking();
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);
        if (orderBy != null)
            query = orderBy(query);
        return await query.ToPaginateAsync(index, size, 0, cancellationToken);
    }

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

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool isSoftDelete = true)
    {
        var hasIsDeleted = typeof(TEntity).GetProperty("IsDeleted") != null;

        if (hasIsDeleted && isSoftDelete)
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