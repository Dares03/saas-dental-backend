using Microsoft.EntityFrameworkCore;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) 
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Multitenancy Global Query Filter
        // Applies to all queries for entities that have a TenantId
        modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<Branch>().HasQueryFilter(b => b.TenantId == _tenantService.GetCurrentTenantId());

        // Configure relationships and constraints
        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a tenant if it has users

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Branches)
            .WithOne(b => b.Tenant)
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.DefaultBranch)
            .WithMany()
            .HasForeignKey(u => u.DefaultBranchId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var currentTenantId = _tenantService.GetCurrentTenantId();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                // Check if the entity has a TenantId property
                var tenantIdProperty = entry.Entity.GetType().GetProperty("TenantId");
                
                if (tenantIdProperty != null && currentTenantId.HasValue)
                {
                    // Only assign if it's currently empty/default (in case it was explicitly set for some reason)
                    var currentValue = tenantIdProperty.GetValue(entry.Entity);
                    if (currentValue == null || (Guid)currentValue == Guid.Empty)
                    {
                        tenantIdProperty.SetValue(entry.Entity, currentTenantId.Value);
                    }
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
