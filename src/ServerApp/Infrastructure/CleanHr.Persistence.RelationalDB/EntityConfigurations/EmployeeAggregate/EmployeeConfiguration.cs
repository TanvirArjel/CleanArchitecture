using CleanHr.Domain.Aggregates.EmployeeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanHr.Persistence.RelationalDB.EntityConfigurations.EmployeeAggregate;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(emp => emp.Id);

        builder.OwnsOne(emp => emp.Name).Property(n => n.FirstName)
            .HasColumnName("FirstName").HasMaxLength(50).IsRequired();

        builder.OwnsOne(emp => emp.Name).Property(n => n.LastName)
            .HasColumnName("LastName").HasMaxLength(50).IsRequired();

        builder.Navigation(emp => emp.Name).IsRequired();

        builder.HasOne(emp => emp.Department).WithMany().HasForeignKey(emp => emp.DepartmentId).IsRequired();

        builder.OwnsOne(emp => emp.DateOfBirth)
            .Property(d => d.Value).HasColumnName("DateOfBirth").HasColumnType("date");
        builder.Navigation(emp => emp.DateOfBirth).IsRequired();

        builder.OwnsOne(emp => emp.Email)
            .Property(e => e.Value).HasColumnName("Email").HasMaxLength(50).IsRequired();
        builder.Navigation(emp => emp.Email).IsRequired();

        builder.OwnsOne(emp => emp.PhoneNumber)
            .Property(p => p.Value).HasColumnName("PhoneNumber").HasMaxLength(15).IsRequired();
        builder.Navigation(emp => emp.PhoneNumber).IsRequired();
    }
}
