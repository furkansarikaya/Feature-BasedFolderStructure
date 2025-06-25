using System.Reflection;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Entities.Orders;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;
using FS.EntityFramework.Library.Extensions;
using FS.EntityFramework.Library.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Context;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditInterceptor auditInterceptor,
    QueryStatisticsInterceptor queryStatisticsInterceptor)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleClaim> RoleClaims => Set<RoleClaim>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserToken> UserTokens => Set<UserToken>();
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Apply global query filters to exclude soft-deleted entities
        builder.ApplySoftDeleteQueryFilters();
        
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditInterceptor);
        optionsBuilder.AddInterceptors(queryStatisticsInterceptor);
    }
}