using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.MaxGuests)
            .IsRequired();

        builder.HasMany(p => p.Reservations)
            .WithOne(r => r.Property)
            .HasForeignKey(r => r.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}