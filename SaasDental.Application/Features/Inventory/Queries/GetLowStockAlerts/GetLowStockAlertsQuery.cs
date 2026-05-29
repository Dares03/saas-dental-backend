using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Queries.GetLowStockAlerts;

public record LowStockAlertDto(
    Guid ProductId,
    string ProductName,
    string SKU,
    string UnitOfMeasure,
    int CurrentStock,
    int MinimumStockAlert);

public record GetLowStockAlertsQuery(Guid BranchId) : IRequest<List<LowStockAlertDto>>;

public class GetLowStockAlertsHandler : IRequestHandler<GetLowStockAlertsQuery, List<LowStockAlertDto>>
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetLowStockAlertsHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<List<LowStockAlertDto>> Handle(GetLowStockAlertsQuery request, CancellationToken cancellationToken)
    {
        var items = await _inventoryRepository.GetLowStockItemsAsync(request.BranchId, cancellationToken);

        return items.Select(i => new LowStockAlertDto(
            i.Product.Id,
            i.Product.Name,
            i.Product.SKU ?? string.Empty,
            i.Product.UnitOfMeasure,
            i.CurrentStock,
            i.Product.MinimumStockAlert
        )).ToList();
    }
}
