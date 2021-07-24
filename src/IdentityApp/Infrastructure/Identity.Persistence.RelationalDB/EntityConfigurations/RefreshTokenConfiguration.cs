using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.RelationalDB.EntityConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(rt => rt.UserId);

            builder.HasOne(rt => rt.ApplicationUser).WithOne(au => au.RefreshToken).HasForeignKey<RefreshToken>(rt => rt.UserId);
            builder.Property(rt => rt.Token).HasMaxLength(100);
            builder.Property(rt => rt.CreatedAtUtc).HasDefaultValueSql("getutcdate()");
        }
    }
}
