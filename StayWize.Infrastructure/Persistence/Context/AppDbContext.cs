using Microsoft.EntityFrameworkCore;
using StayWize.Domain.Common;
using StayWize.Domain.Entities;

namespace StayWize.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<HostLocal> HostLocals => Set<HostLocal>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<AccessCode> AccessCodes => Set<AccessCode>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones del assembly automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Filtro global de soft delete para todas las entidades que hereden BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(
                        System.Linq.Expressions.Expression.Lambda(
                            System.Linq.Expressions.Expression.Equal(
                                System.Linq.Expressions.Expression.Property(
                                    System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e"),
                                    nameof(BaseEntity.IsDeleted)),
                                System.Linq.Expressions.Expression.Constant(false)),
                            System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e")));
            }
        }
    }
}