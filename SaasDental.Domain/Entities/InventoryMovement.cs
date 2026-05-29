using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public MovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }

    public Guid InventoryItemId { get; private set; }
    public InventoryItem InventoryItem { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid TenantId { get; private set; }

    private InventoryMovement() { }

    public InventoryMovement(MovementType type, int quantity, string reason, Guid inventoryItemId, Guid userId, Guid tenantId)
    {
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");

        Type = type;
        Quantity = quantity;
        Reason = reason;
        Date = DateTime.UtcNow;
        InventoryItemId = inventoryItemId;
        UserId = userId;
        TenantId = tenantId;
    }
}
