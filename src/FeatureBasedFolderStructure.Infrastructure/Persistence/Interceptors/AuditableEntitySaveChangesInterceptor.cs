using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Common;
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
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUserService.UserId;
                    entry.Entity.CreatedAt = dateTime.Now;
                    break;
                case EntityState.Modified:
                    if (entry.Property("IsDeleted").CurrentValue is true &&
                        entry.Property("IsDeleted").OriginalValue is false)
                    {
                        entry.Entity.DeletedBy = currentUserService.UserId;
                        entry.Entity.DeletedAt = dateTime.Now;
                    }
                    else
                    {
                        entry.Entity.UpdatedBy = currentUserService.UserId;
                        entry.Entity.UpdatedAt = dateTime.Now;
                    }

                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted: 
                default:
                    break;
            }
        }
    }
}