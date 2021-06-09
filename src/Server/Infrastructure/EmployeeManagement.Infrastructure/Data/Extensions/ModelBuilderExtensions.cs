using System;
using System.Linq;
using System.Reflection;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyBaseEntityConfiguration(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            MethodInfo configureMethod = typeof(BaseEntityConfiguration).GetTypeInfo().DeclaredMethods
                .Single(m => m.Name == nameof(BaseEntityConfiguration.Configure));

            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
                {
                    configureMethod.MakeGenericMethod(entityType.ClrType).Invoke(null, new[] { modelBuilder });
                }
            }

            return modelBuilder;
        }
    }
}
