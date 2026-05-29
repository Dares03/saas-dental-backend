using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? SKU { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty; // e.g. "Caja", "Unidad", "Litro"
    public int MinimumStockAlert { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid CategoryId { get; private set; }
    public ProductCategory Category { get; private set; } = null!;

    public Guid TenantId { get; private set; }

    public ICollection<InventoryItem> InventoryItems { get; private set; } = new List<InventoryItem>();

    private Product() { }

    public Product(string name, string? description, string? sku, string unitOfMeasure, int minimumStockAlert, Guid categoryId, Guid tenantId)
    {
        Name = name;
        Description = description;
        SKU = sku;
        UnitOfMeasure = unitOfMeasure;
        MinimumStockAlert = minimumStockAlert;
        CategoryId = categoryId;
        TenantId = tenantId;
    }

    public void UpdateDetails(string name, string? description, string? sku, string unitOfMeasure, int minimumStockAlert, Guid categoryId)
    {
        Name = name;
        Description = description;
        SKU = sku;
        UnitOfMeasure = unitOfMeasure;
        MinimumStockAlert = minimumStockAlert;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
