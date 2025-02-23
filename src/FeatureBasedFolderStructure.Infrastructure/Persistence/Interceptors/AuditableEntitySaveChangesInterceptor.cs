using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Common;
using FeatureBasedFolderStructure.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor(
    ICurrentUserService currentUserService,
    IDateTime dateTime)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity<int>>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUserService.UserId;
                entry.Entity.CreatedAt = dateTime.Now;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedBy = currentUserService.UserId;
                entry.Entity.UpdatedAt = dateTime.Now;
            }
        }
    }
}