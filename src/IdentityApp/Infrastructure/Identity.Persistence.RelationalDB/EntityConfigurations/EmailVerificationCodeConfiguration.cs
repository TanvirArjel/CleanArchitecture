using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.RelationalDB.EntityConfigurations;

public class EmailVerificationCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
        builder.ToTable("EmailVerificationCodes");
        builder.HasKey(evc => evc.Id);
        builder.Property(evc => evc.Id).ValueGeneratedOnAdd();

        builder.Property(evc => evc.Email).HasMaxLength(50).IsRequired();
        builder.Property(evc => evc.Code).HasMaxLength(6).IsFixedLength().IsRequired();

        builder.Property(eu => eu.SentAtUtc).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(eu => eu.UsedAtUtc).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Throw);
    }
}
