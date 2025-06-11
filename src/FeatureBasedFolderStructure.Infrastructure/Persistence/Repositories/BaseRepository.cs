using System.Linq.Expressions;
using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Common.Models;
using FeatureBasedFolderStructure.Domain.Specifications;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Paging;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<TEntity, TKey>(DbContext context) : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, bool enableTracking = false) =>
        enableTracking
            ? await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
            : await _dbSet.FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (disableTracking) 
            query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool isSoftDelete = true)
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

        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetWithIncludeStringAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (disableTracking) 
            query = query.AsNoTracking();
            
        query = query.ApplySpecification(predicate)
                     .ApplyInclude(includeString)
                     .ApplyOrder(orderBy);
                     
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (disableTracking) 
            query = query.AsNoTracking();
            
        query = query.ApplySpecification(predicate)
                     .ApplyInclude(includes)
                     .ApplyOrder(orderBy);
                     
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAsync(BaseSpecification<TEntity> spec, CancellationToken cancellationToken = default) => await ApplySpecification(spec).ToListAsync(cancellationToken);

    public virtual async Task<IPaginate<TEntity>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (disableTracking) 
            query = query.AsNoTracking();
            
        query = query.ApplySpecification(predicate)
                     .ApplyInclude(includes)
                     .ApplyOrder(orderBy);

        return await query.ToPaginateAsync(pageIndex, pageSize, 0, cancellationToken);
    }

    public virtual async Task<IPaginate<TEntity>> GetPagedWithFilterAsync(
        FilterModel filter,
        int pageIndex,
        int pageSize,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true, 
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (disableTracking) 
            query = query.AsNoTracking();
            
        query = query.ApplyFilter(filter)
                     .ApplyInclude(includes)
                     .ApplyOrder(orderBy);

        return await query.ToPaginateAsync(pageIndex, pageSize, 0, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query.CountAsync(cancellationToken);
    }

    public virtual IQueryable<TEntity> GetQueryable(bool disableTracking = true) => disableTracking ? _dbSet.AsNoTracking() : _dbSet;

    public async Task<int> ExecuteNativeQueryAsync(string query, params object[] parameters) => await context.Database.ExecuteSqlRawAsync(query, parameters);

    private IQueryable<TEntity> ApplySpecification(BaseSpecification<TEntity> spec)
    {
        var query = GetQueryable();
        
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);
            
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
        
        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
            
        if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);
            
        if (spec.GroupBy != null)
            query = query.GroupBy(spec.GroupBy).SelectMany(x => x);
            
        if (spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);
            
        return query;
    }
}
// {
//     private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
//
//     public virtual IQueryable<TEntity> AsQueryable() => _dbSet;
//
//     public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, bool enableTracking = false) =>
//         enableTracking
//             ? await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
//             : await _dbSet.FindAsync([id], cancellationToken);
//
//     public async Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
//         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
//         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
//         int index = 0, int size = 10,
//         bool enableTracking = true,
//         CancellationToken cancellationToken = default)
//     {
//         var query = AsQueryable();
//         if (enableTracking) query = query.AsTracking();
//         if (include != null) query = include(query);
//         if (predicate != null) query = query.Where(predicate);
//         if (orderBy != null)
//             query = orderBy(query);
//         return await query.ToPaginateAsync(index, size, 0, cancellationToken);
//     }
//
//     public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbSet.ToListAsync(cancellationToken);
//
//     public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
//     {
//         await _dbSet.AddAsync(entity, cancellationToken);
//         await context.SaveChangesAsync(cancellationToken);
//         return entity;
//     }
//
//     public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
//     {
//         await _dbSet.AddRangeAsync(entities, cancellationToken);
//         await context.SaveChangesAsync(cancellationToken);
//         return entities;
//     }
//
//     public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
//     {
//         context.Entry(entity).State = EntityState.Modified;
//         await context.SaveChangesAsync(cancellationToken);
//     }
//
//     public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
//     {
//         foreach (var entity in entities) 
//             context.Entry(entity).State = EntityState.Modified;
//
//         await context.SaveChangesAsync(cancellationToken);
//     }
//
//     public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool isSoftDelete = true)
//     {
//         var hasIsDeleted = typeof(TEntity).GetProperty("IsDeleted") != null;
//
//         if (hasIsDeleted && isSoftDelete)
//         {
//             var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
//             isDeletedProperty!.SetValue(entity, true);
//             context.Entry(entity).State = EntityState.Modified;
//         }
//         else
//             _dbSet.Remove(entity);
//
//         await context.SaveChangesAsync(cancellationToken);
//     }
//
//     public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool isSoftDelete = true)
//     {
//         var hasIsDeleted = typeof(TEntity).GetProperty("IsDeleted") != null;
//
//         if (hasIsDeleted && isSoftDelete)
//         {
//             var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
//             foreach (var entity in entities)
//             {
//                 isDeletedProperty!.SetValue(entity, true);
//                 context.Entry(entity).State = EntityState.Modified;
//             }
//         }
//         else
//             _dbSet.RemoveRange(entities);
//
//         await context.SaveChangesAsync(cancellationToken);
//     }
// }