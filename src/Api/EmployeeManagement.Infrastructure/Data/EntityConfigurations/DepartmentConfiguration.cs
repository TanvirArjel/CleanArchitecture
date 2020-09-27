using System;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Data.EntityConfigurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ToTable("Departments");
            builder.HasKey(d => d.DepartmentId);
            builder.Property(d => d.DepartmentName).HasMaxLength(50).IsRequired();
            builder.Property(d => d.Description).HasMaxLength(200).IsRequired();

            builder.HasIndex(d => d.DepartmentName).IsUnique();
        }
    }
}
