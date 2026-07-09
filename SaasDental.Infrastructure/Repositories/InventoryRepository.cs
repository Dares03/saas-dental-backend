using Microsoft.EntityFrameworkCore;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Infrastructure.Persistence;

namespace SaasDental.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<InventoryItem?> GetInventoryItemAsync(Guid productId, Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InventoryItems
            .FirstOrDefaultAsync(ii => ii.ProductId == productId && ii.BranchId == branchId, cancellationToken);
    }

    public async Task AddInventoryItemAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default)
    {
        await _dbContext.InventoryItems.AddAsync(inventoryItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateInventoryItemAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryItems.Update(inventoryItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<InventoryItem>> GetAllItemsByBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InventoryItems
            .Include(ii => ii.Product)
                .ThenInclude(p => p.Category)
            .Where(ii => ii.BranchId == branchId)
            .OrderBy(ii => ii.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductCategory>> GetProductCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductCategory>()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryItem>> GetLowStockItemsAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InventoryItems
            .Include(ii => ii.Product)
            .Where(ii => ii.BranchId == branchId && ii.CurrentStock <= ii.Product.MinimumStockAlert)
            .ToListAsync(cancellationToken);
    }

    public async Task AddInventoryMovementAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
    {
        await _dbContext.InventoryMovements.AddAsync(movement, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
