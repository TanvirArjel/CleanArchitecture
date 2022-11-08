////using System;
////using EmployeeManagement.Domain.Entities;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.EntityFrameworkCore.Metadata;

////namespace EmployeeManagement.Persistence.RelationalDB.EntityConfigurations.DomainEntities;

////public static class BaseEntityConfiguration
////{
////    public static void Configure<TEntity>(ModelBuilder modelBuilder)
////        where TEntity : Entity
////    {
////        if (modelBuilder == null)
////        {
////            throw new ArgumentNullException(nameof(modelBuilder));
////        }

////        modelBuilder.Entity<TEntity>(builder =>
////        {
////            builder.Property<int>("IdentityKey").ValueGeneratedOnAdd();

////            builder.HasKey(cr => cr.Id);
////            builder.Property(cr => cr.CreatedAtUtc).HasDefaultValueSql("getutcdate()");
////            ////builder.Property(cr => cr.CreatedAtUtc).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
////            builder.Property(cr => cr.CreatedAtUtc).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

////            builder.Property(cr => cr.LastModifiedAtUtc).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
////        });
////    }
////}
