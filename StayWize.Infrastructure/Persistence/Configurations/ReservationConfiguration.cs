using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.GuestCount).IsRequired();
        builder.Property(r => r.CheckIn).IsRequired();
        builder.Property(r => r.CheckOut).IsRequired();
        builder.Property(r => r.Status).IsRequired();
        builder.Property(r => r.Notes).HasMaxLength(1000);

        builder.HasOne(r => r.HostLocal)
            .WithMany()
            .HasForeignKey(r => r.HostLocalId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(r => r.AccessCodes)
            .WithOne(a => a.Reservation)
            .HasForeignKey(a => a.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}