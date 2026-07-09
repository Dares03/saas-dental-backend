using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Queries.GetInventoryItems;

public record InventoryItemDto(
    Guid InventoryItemId,
    Guid ProductId,
    string Name,
    string? Description,
    string Category,
    int CurrentStock,
    int MinimumStockAlert,
    string UnitOfMeasure,
    string? SKU,
    DateTime LastUpdated);

public record GetInventoryItemsQuery(Guid BranchId) : IRequest<List<InventoryItemDto>>;

public class GetInventoryItemsHandler : IRequestHandler<GetInventoryItemsQuery, List<InventoryItemDto>>
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetInventoryItemsHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<List<InventoryItemDto>> Handle(GetInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _inventoryRepository.GetAllItemsByBranchAsync(request.BranchId, cancellationToken);

        return items.Select(i => new InventoryItemDto(
            i.Id,
            i.Product.Id,
            i.Product.Name,
            i.Product.Description,
            i.Product.Category?.Name ?? "Sin categoría",
            i.CurrentStock,
            i.Product.MinimumStockAlert,
            i.Product.UnitOfMeasure,
            i.Product.SKU,
            i.UpdatedAt ?? i.CreatedAt
        )).ToList();
    }
}
