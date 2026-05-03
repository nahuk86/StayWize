using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Configurations;

public class AccessCodeConfiguration : IEntityTypeConfiguration<AccessCode>
{
    public void Configure(EntityTypeBuilder<AccessCode> builder)
    {
        builder.ToTable("AccessCodes");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.ValidFrom).IsRequired();
        builder.Property(a => a.ValidTo).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.Type).IsRequired();

        builder.HasIndex(a => a.Code).IsUnique();

        builder.HasMany(a => a.AccessLogs)
            .WithOne(l => l.AccessCode)
            .HasForeignKey(l => l.AccessCodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}