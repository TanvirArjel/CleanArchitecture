using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Persistence.RelationalDB.EntityConfigurations.IdentityAggregate;

internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        ////builder.Property<int>("IdentityKey").ValueGeneratedOnAdd();
    }
}
