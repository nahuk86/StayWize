using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Configurations;

public class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("AccessLogs");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EventType).IsRequired();
        builder.Property(a => a.EventTime).IsRequired();
        builder.Property(a => a.Success).IsRequired();

        builder.Property(a => a.FailureReason)
            .HasMaxLength(500);
    }
}