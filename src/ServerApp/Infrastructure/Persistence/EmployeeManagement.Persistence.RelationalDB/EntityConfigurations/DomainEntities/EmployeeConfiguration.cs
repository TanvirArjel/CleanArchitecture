using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Persistence.RelationalDB.EntityConfigurations.DomainEntities
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(emp => emp.Id);

            builder.Property(emp => emp.Name).HasMaxLength(50).IsRequired();
            builder.HasOne(emp => emp.Department).WithMany().HasForeignKey(emp => emp.DepartmentId).IsRequired();
            builder.Property(emp => emp.DateOfBirth).HasColumnType("date");
            builder.Property(emp => emp.Email).HasMaxLength(50).IsRequired();
            builder.Property(emp => emp.PhoneNumber).HasMaxLength(15).IsRequired();
        }
    }
}
