////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Reflection;
////using EmployeeManagement.Domain.Entities;
////using EmployeeManagement.Persistence.RelationalDB.EntityConfigurations.DomainEntities;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.EntityFrameworkCore.ChangeTracking;

////namespace EmployeeManagement.Persistence.RelationalDB.Extensions;

////internal static class ModelBuilderExtensions
////{
////    public static ModelBuilder ApplyBaseEntityConfiguration(this ModelBuilder modelBuilder)
////    {
////        if (modelBuilder == null)
////        {
////            throw new ArgumentNullException(nameof(modelBuilder));
////        }

////        MethodInfo configureMethod = typeof(BaseEntityConfiguration).GetTypeInfo().DeclaredMethods
////            .Single(m => m.Name == nameof(BaseEntityConfiguration.Configure));

////        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
////        {
////            if (entityType.ClrType.IsSubclassOf(typeof(Entity)))
////            {
////                configureMethod.MakeGenericMethod(entityType.ClrType).Invoke(null, new[] { modelBuilder });
////            }
////        }

////        return modelBuilder;
////    }

////    public static void ApplyValueGenerationOnUpdate(this ChangeTracker changeTracker)
////    {
////        if (changeTracker == null)
////        {
////            throw new ArgumentNullException(nameof(changeTracker));
////        }

////        List<EntityEntry<Entity>> modifiedEntries = changeTracker.Entries<Entity>()
////            .Where(e => e.State == EntityState.Modified).ToList();

////        if (modifiedEntries.Any())
////        {
////            foreach (EntityEntry<Entity> entityEntry in modifiedEntries)
////            {
////                int changePropertiesCount = entityEntry.Properties.Where(p => p.IsModified).Count();

////                if (changePropertiesCount > 0)
////                {
////                    bool isMenuallySet = entityEntry.Property(p => p.LastModifiedAtUtc).IsModified;
////                    if (isMenuallySet)
////                    {
////                        throw new InvalidOperationException("You can't set the value for LastModifiedAtUtc property. The value of this property will be set by system.");
////                    }

////                    entityEntry.Property(p => p.LastModifiedAtUtc).CurrentValue = DateTime.UtcNow;
////                }
////            }
////        }
////    }
////}
