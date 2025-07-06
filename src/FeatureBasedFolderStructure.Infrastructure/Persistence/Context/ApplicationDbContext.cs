using System.Reflection;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Entities.Orders;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Infrastructure.Features.Common.Interceptors;
using FS.EntityFramework.Library.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Context;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
    : FSDbContext(options, serviceProvider)
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
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var queryStatisticsInterceptor = serviceProvider.CreateScope().ServiceProvider.GetService<QueryStatisticsInterceptor>();
        if (queryStatisticsInterceptor != null)
        {
            optionsBuilder.AddInterceptors(queryStatisticsInterceptor);
        }
    }
}