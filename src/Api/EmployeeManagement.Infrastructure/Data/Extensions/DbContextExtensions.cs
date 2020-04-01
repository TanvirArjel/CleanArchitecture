using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace EmployeeManagement.Infrastructure.Data.Extensions
{
    public static class DbContextExtensions
    {
        public static void ApplyValueGenerationOnUpdate(this ChangeTracker changeTracker)
        {
            if (changeTracker == null)
            {
                throw new ArgumentNullException(nameof(changeTracker));
            }

            var modifiedEntries = changeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified).ToList();

            if (modifiedEntries.Any())
            {
                foreach (EntityEntry<BaseEntity> entityEntry in modifiedEntries)
                {
                    bool hasAnyPropertyValueModified = entityEntry.Properties.Any(p => p.IsModified);

                    if (hasAnyPropertyValueModified)
                    {
                        bool isMenuallySet = entityEntry.Property(p => p.LastModifiedAtUtc).IsModified;
                        if (isMenuallySet)
                        {
                            throw new ApplicationException("You can't set the value for LastModifiedAtUtc property. The value of this property will be set by system.");
                        }

                        entityEntry.Property(p => p.LastModifiedAtUtc).CurrentValue = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
