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
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientRelative> PatientRelatives => Set<PatientRelative>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    
    // Clinical Module
    public DbSet<ClinicalHistory> ClinicalHistories => Set<ClinicalHistory>();
    public DbSet<Odontogram> Odontograms => Set<Odontogram>();
    public DbSet<Tooth> Teeth => Set<Tooth>();
    public DbSet<ToothSurface> ToothSurfaces => Set<ToothSurface>();
    public DbSet<ClinicalFinding> ClinicalFindings => Set<ClinicalFinding>();

    // Financial Module
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<TreatmentService> TreatmentServices => Set<TreatmentService>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();
    public DbSet<PatientDebt> PatientDebts => Set<PatientDebt>();

    // Inventory Module
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Multitenancy Global Query Filter
        // Applies to all queries for entities that have a TenantId
        modelBuilder.Entity<User>().HasQueryFilter(u => u.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<Branch>().HasQueryFilter(b => b.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<Patient>().HasQueryFilter(p => p.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<Appointment>().HasQueryFilter(a => a.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<ServiceCategory>().HasQueryFilter(sc => sc.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<TreatmentService>().HasQueryFilter(ts => ts.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<CashRegister>().HasQueryFilter(cr => cr.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<CashTransaction>().HasQueryFilter(ct => ct.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<PatientDebt>().HasQueryFilter(pd => pd.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<ProductCategory>().HasQueryFilter(pc => pc.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<InventoryItem>().HasQueryFilter(ii => ii.TenantId == _tenantService.GetCurrentTenantId());
        modelBuilder.Entity<InventoryMovement>().HasQueryFilter(im => im.TenantId == _tenantService.GetCurrentTenantId());

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

        // Patient Configurations
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Relatives)
            .WithOne(pr => pr.Patient)
            .HasForeignKey(pr => pr.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasIndex(p => new { p.TenantId, p.DocumentId })
            .IsUnique(); // Cannot have the same document ID twice in the same clinic

        // Appointment Configurations
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Dentist)
            .WithMany()
            .HasForeignKey(a => a.DentistId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Branch)
            .WithMany()
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Clinical History Configurations
        modelBuilder.Entity<ClinicalHistory>()
            .HasOne(ch => ch.Patient)
            .WithOne()
            .HasForeignKey<ClinicalHistory>(ch => ch.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Odontogram>()
            .HasOne(o => o.ClinicalHistory)
            .WithMany(ch => ch.Odontograms)
            .HasForeignKey(o => o.ClinicalHistoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tooth>()
            .HasOne(t => t.Odontogram)
            .WithMany(o => o.Teeth)
            .HasForeignKey(t => t.OdontogramId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ToothSurface>()
            .HasOne(ts => ts.Tooth)
            .WithMany(t => t.Surfaces)
            .HasForeignKey(ts => ts.ToothId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClinicalFinding>()
            .HasOne(cf => cf.Tooth)
            .WithMany(t => t.ToothLevelFindings)
            .HasForeignKey(cf => cf.ToothId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClinicalFinding>()
            .HasOne(cf => cf.ToothSurface)
            .WithMany(ts => ts.Findings)
            .HasForeignKey(cf => cf.ToothSurfaceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Financial Configurations
        modelBuilder.Entity<TreatmentService>()
            .HasOne(ts => ts.Category)
            .WithMany(sc => sc.Services)
            .HasForeignKey(ts => ts.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CashRegister>()
            .HasOne(cr => cr.Branch)
            .WithMany()
            .HasForeignKey(cr => cr.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CashRegister>()
            .HasOne(cr => cr.OpenedByUser)
            .WithMany()
            .HasForeignKey(cr => cr.OpenedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CashTransaction>()
            .HasOne(ct => ct.CashRegister)
            .WithMany(cr => cr.Transactions)
            .HasForeignKey(ct => ct.CashRegisterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CashTransaction>()
            .HasOne(ct => ct.PatientDebt)
            .WithMany()
            .HasForeignKey(ct => ct.PatientDebtId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PatientDebt>()
            .HasOne(pd => pd.Patient)
            .WithMany()
            .HasForeignKey(pd => pd.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PatientDebt>()
            .HasOne(pd => pd.Appointment)
            .WithMany()
            .HasForeignKey(pd => pd.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Inventory Configurations
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(pc => pc.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Product)
            .WithMany(p => p.InventoryItems)
            .HasForeignKey(ii => ii.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Branch)
            .WithMany()
            .HasForeignKey(ii => ii.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryMovement>()
            .HasOne(im => im.InventoryItem)
            .WithMany(ii => ii.Movements)
            .HasForeignKey(im => im.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventoryMovement>()
            .HasOne(im => im.User)
            .WithMany()
            .HasForeignKey(im => im.UserId)
            .OnDelete(DeleteBehavior.Restrict);
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
