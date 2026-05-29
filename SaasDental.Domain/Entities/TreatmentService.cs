using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class TreatmentService : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal BasePrice { get; private set; }
    public decimal DoctorCommissionPercentage { get; private set; } // e.g. 40.0 for 40%
    public bool IsActive { get; private set; } = true;

    public Guid ServiceCategoryId { get; private set; }
    public ServiceCategory Category { get; private set; } = null!;

    public Guid TenantId { get; private set; }

    private TreatmentService() { }

    public TreatmentService(string name, string? description, decimal basePrice, decimal doctorCommissionPercentage, Guid serviceCategoryId, Guid tenantId)
    {
        Name = name;
        Description = description;
        BasePrice = basePrice;
        DoctorCommissionPercentage = doctorCommissionPercentage;
        ServiceCategoryId = serviceCategoryId;
        TenantId = tenantId;
    }

    public void UpdateDetails(string name, string? description, decimal basePrice, decimal doctorCommissionPercentage, Guid serviceCategoryId)
    {
        Name = name;
        Description = description;
        BasePrice = basePrice;
        DoctorCommissionPercentage = doctorCommissionPercentage;
        ServiceCategoryId = serviceCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
