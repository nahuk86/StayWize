using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StayWize.Domain.Common;
using StayWize.Domain.Entities;
using StayWize.Services.Authentication;

namespace StayWize.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<HostLocal> HostLocals => Set<HostLocal>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<AccessCode> AccessCodes => Set<AccessCode>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();
    public DbSet<PropertyHostLocal> PropertyHostLocals => Set<PropertyHostLocal>();
    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();
    public DbSet<ClientRegistrationRequest> ClientRegistrationRequests => Set<ClientRegistrationRequest>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<PropertyHostLocal>(entity =>
        {
            entity.HasKey(e => new { e.PropertyId, e.HostLocalId });

            entity.HasOne(e => e.Property)
                .WithMany(p => p.HostLocalAssignments)
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.HostLocal)
                .WithMany(h => h.PropertyAssignments)
                .HasForeignKey(e => e.HostLocalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserInvitation>(entity =>
        {
            entity.HasIndex(e => e.TokenHash).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.Property(e => e.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
            }

            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.RowVersion))
                    .IsRowVersion();
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression BuildSoftDeleteFilter(Type type)
    {
        var param = System.Linq.Expressions.Expression.Parameter(type, "e");
        var prop = System.Linq.Expressions.Expression.Property(param, nameof(ISoftDeletable.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(
            prop,
            System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, param);
    }
}