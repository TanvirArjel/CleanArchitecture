using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace EmployeeManagement.Infrastructure.Data.EntityConfigurations
{
    public static class BaseEntityConfiguration
    {
        public static void Configure<TEntity>(ModelBuilder modelBuilder)
            where TEntity : BaseEntity
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<TEntity>(builder =>
            {
                builder.Property(cr => cr.CreatedAtUtc).HasDefaultValueSql("getutcdate()");
                builder.Property(cr => cr.CreatedAtUtc).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
                builder.Property(cr => cr.CreatedAtUtc).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

                builder.Property(cr => cr.LastModifiedAtUtc).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
            });
        }
    }
}
