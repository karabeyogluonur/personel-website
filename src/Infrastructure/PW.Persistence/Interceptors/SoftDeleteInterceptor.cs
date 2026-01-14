using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PW.Domain.Common;

namespace TM.Infrastructure.Persistence.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
   private readonly TimeProvider _timeProvider;

   public SoftDeleteInterceptor(
       TimeProvider timeProvider)
   {
      _timeProvider = timeProvider;
   }

   public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
   {
      UpdateSoftDeleteEntities(eventData.Context);
      return base.SavingChanges(eventData, result);
   }

   public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
   {
      UpdateSoftDeleteEntities(eventData.Context);
      return base.SavingChangesAsync(eventData, result, cancellationToken);
   }

   private void UpdateSoftDeleteEntities(DbContext? context)
   {
      if (context == null) return;

      var now = _timeProvider.GetUtcNow().UtcDateTime;

      foreach (var entry in context.ChangeTracker.Entries<ISoftDeleteEntity>())
      {
         if (entry.State == EntityState.Deleted)
         {
            entry.State = EntityState.Modified;

            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = now;
         }
      }
   }
}
