using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PW.Domain.Common;

namespace PW.Persistence.Interceptors;

public class AuditableInterceptor : SaveChangesInterceptor
{
   private readonly TimeProvider _timeProvider;

   public AuditableInterceptor(
       TimeProvider timeProvider)
   {
      _timeProvider = timeProvider;
   }

   public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
   {
      UpdateAuditableEntities(eventData.Context);
      return base.SavingChanges(eventData, result);
   }

   public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
   {
      UpdateAuditableEntities(eventData.Context);
      return base.SavingChangesAsync(eventData, result, cancellationToken);
   }

   private void UpdateAuditableEntities(DbContext? context)
   {
      if (context == null) return;

      var now = _timeProvider.GetUtcNow().UtcDateTime;

      foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
      {
         if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) continue;

         if (entry.State == EntityState.Added)
            entry.Entity.CreatedAt = now;

         if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            entry.Entity.UpdatedAt = now;
      }
   }
}


public static class Extensions
{
   public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
       entry.References.Any(r =>
           r.TargetEntry != null &&
           r.TargetEntry.Metadata.IsOwned() &&
           (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
