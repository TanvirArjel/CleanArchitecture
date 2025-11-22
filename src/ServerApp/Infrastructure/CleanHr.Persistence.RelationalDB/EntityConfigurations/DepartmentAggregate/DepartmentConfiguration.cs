using System;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanHr.Persistence.RelationalDB.EntityConfigurations.DepartmentAggregate;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Departments");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(d => d.Name).IsUnique();

        builder.Property(d => d.Description).HasMaxLength(200).IsRequired();
    }
}
