using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Persistence.Indentity.EntityConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property<int>("IdentityKey").ValueGeneratedOnAdd();

            builder.Property(au => au.FirstName).HasMaxLength(40).IsRequired();
            builder.Property(au => au.LastName).HasMaxLength(30).IsRequired();


            builder.Property(au => au.UserName).HasMaxLength(50).IsRequired();
            builder.Property(au => au.NormalizedUserName).HasMaxLength(50).IsRequired();
            builder.Property(au => au.Email).HasMaxLength(50).IsRequired();
            builder.Property(au => au.NormalizedEmail).HasMaxLength(50).IsRequired();
            builder.Property(au => au.PhoneNumber).HasMaxLength(15).IsRequired(false);
            builder.Property(au => au.DialCode).HasMaxLength(4).IsRequired(false);
            builder.Property(au => au.LanguageCulture).HasMaxLength(4).IsRequired(false);

            builder.HasIndex(au => au.Email).IsUnique(true);
            builder.HasIndex(au => au.NormalizedEmail).IsUnique(true);
        }
    }
}
