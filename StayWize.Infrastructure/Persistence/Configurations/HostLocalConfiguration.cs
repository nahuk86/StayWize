using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Configurations;

public class HostLocalConfiguration : IEntityTypeConfiguration<HostLocal>
{
    public void Configure(EntityTypeBuilder<HostLocal> builder)
    {
        builder.ToTable("HostLocals");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(h => h.Zone)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(h => h.Email).IsUnique();
    }
}