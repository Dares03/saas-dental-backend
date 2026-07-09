using SaasDental.Domain.Entities;

namespace SaasDental.Application.Common.Interfaces;

public interface IInventoryRepository
{
    // Products
    Task AddProductAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<ProductCategory>> GetProductCategoriesAsync(CancellationToken cancellationToken = default);
    
    // Inventory Items (Stock per branch)
    Task<InventoryItem?> GetInventoryItemAsync(Guid productId, Guid branchId, CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetAllItemsByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task AddInventoryItemAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task UpdateInventoryItemAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetLowStockItemsAsync(Guid branchId, CancellationToken cancellationToken = default);

    // Movements (Kardex)
    Task AddInventoryMovementAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
}
