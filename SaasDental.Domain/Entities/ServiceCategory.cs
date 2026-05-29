using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class ServiceCategory : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public Guid TenantId { get; private set; }

    public ICollection<TreatmentService> Services { get; private set; } = new List<TreatmentService>();

    private ServiceCategory() { }

    public ServiceCategory(string name, string? description, Guid tenantId)
    {
        Name = name;
        Description = description;
        TenantId = tenantId;
    }

    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
