using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid TenantId { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private ProductCategory() { }

    public ProductCategory(string name, string? description, Guid tenantId)
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

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
