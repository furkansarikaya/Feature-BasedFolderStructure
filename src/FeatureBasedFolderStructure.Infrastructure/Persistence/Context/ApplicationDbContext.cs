using System.Reflection;
using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.ValueObjects;
using FeatureBasedFolderStructure.Infrastructure.Extensions;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Context;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IMediator mediator,
    AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Ignore<DomainEvent>();
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await mediator.DispatchDomainEvents(this);
        return await base.SaveChangesAsync(cancellationToken);
    }
}

//cd src/FeatureBasedFolderStructure.Infrastructure
//dotnet ef migrations add InitialCreate --startup-project ../FeatureBasedFolderStructure.API
//dotnet ef database update --startup-project ../FeatureBasedFolderStructure.API