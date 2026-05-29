using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public int CurrentStock { get; private set; }

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = null!;

    public Guid TenantId { get; private set; }

    public ICollection<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

    private InventoryItem() { }

    public InventoryItem(Guid productId, Guid branchId, Guid tenantId)
    {
        ProductId = productId;
        BranchId = branchId;
        TenantId = tenantId;
        CurrentStock = 0;
    }

    public void ApplyMovement(int quantity, MovementType type)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad del movimiento debe ser mayor a cero.");

        if (type == MovementType.Entry)
        {
            CurrentStock += quantity;
        }
        else if (type == MovementType.Exit)
        {
            if (CurrentStock < quantity)
                throw new InvalidOperationException("No hay suficiente stock para realizar esta salida.");
                
            CurrentStock -= quantity;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
