using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanHr.Persistence.RelationalDB.EntityConfigurations.IdentityAggregate;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);

        builder.HasOne(rt => rt.ApplicationUser)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(rt => rt.Token).HasMaxLength(100).IsRequired();
        builder.Property(rt => rt.TokenFamily).HasMaxLength(50).IsRequired();
        builder.Property(rt => rt.IsRevoked).IsRequired();
        builder.Property(rt => rt.CreatedAtUtc).HasDefaultValueSql("getutcdate()");

        // Create index for faster token lookups
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => new { rt.UserId, rt.TokenFamily });
        builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked });
    }
}
