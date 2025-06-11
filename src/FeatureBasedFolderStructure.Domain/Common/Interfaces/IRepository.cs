using System.Linq.Expressions;
using FeatureBasedFolderStructure.Domain.Common.Models;
using FeatureBasedFolderStructure.Domain.Specifications;
using Microsoft.EntityFrameworkCore.Query;

namespace FeatureBasedFolderStructure.Domain.Common.Interfaces;

public interface IRepository<TEntity, TKey> 
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    // IQueryable<TEntity> AsQueryable();
    // Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, bool enableTracking = false);
    // Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
    //     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    //     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    //     int index = 0, int size = 10,
    //     bool enableTracking = true,
    //     CancellationToken cancellationToken = default);
    // Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    // Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    // Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    // Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    // Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    // Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool isSoftDelete = true);
    // Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool isSoftDelete = true);
    
    // Temel CRUD Operasyonları
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, bool enableTracking = false);
    Task<IReadOnlyList<TEntity>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool isSoftDelete = true);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Gelişmiş Sorgulama
    Task<IReadOnlyList<TEntity>> GetWithIncludeStringAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);
    
    // Specification pattern kullanarak sorgulama
    Task<IReadOnlyList<TEntity>> GetAsync(BaseSpecification<TEntity> spec,
        CancellationToken cancellationToken = default);
    
    // Sayfalama için
    Task<IPaginate<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);
    
    // Dinamik Filtreleme
    Task<IPaginate<TEntity>> GetPagedWithFilterAsync(
        FilterModel filter,
        int pageIndex,
        int pageSize,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);
    
    // Sayım
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    // Raw IQueryable (ileri düzey LINQ sorguları için)
    IQueryable<TEntity> GetQueryable(bool disableTracking = true);
    
    // SQL Sorguları
    Task<int> ExecuteNativeQueryAsync(string query, params object[] parameters);
}