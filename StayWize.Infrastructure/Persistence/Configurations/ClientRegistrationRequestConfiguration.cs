using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Configurations;

public class ClientRegistrationRequestConfiguration
    : IEntityTypeConfiguration<ClientRegistrationRequest>
{
    public void Configure(EntityTypeBuilder<ClientRegistrationRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Phone)
            .HasMaxLength(50);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(r => r.Email);
        builder.HasIndex(r => r.DocumentNumber);
        builder.HasIndex(r => r.Status);
    }
}