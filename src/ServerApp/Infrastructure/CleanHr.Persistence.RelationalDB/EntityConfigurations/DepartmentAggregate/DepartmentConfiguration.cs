using System;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanHr.Persistence.RelationalDB.EntityConfigurations.DepartmentAggregate;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.ToTable("Departments");
        builder.HasKey(d => d.Id);

        builder.OwnsOne(d => d.Name, navBuilder =>
        {
            navBuilder.Property(n => n.Value).HasMaxLength(50).HasColumnName("Name").IsRequired();
            navBuilder.HasIndex(n => n.Value).IsUnique();
        });

        builder.Navigation(d => d.Name).IsRequired();

        builder.Property(d => d.Description).HasMaxLength(200).IsRequired();
    }
}
