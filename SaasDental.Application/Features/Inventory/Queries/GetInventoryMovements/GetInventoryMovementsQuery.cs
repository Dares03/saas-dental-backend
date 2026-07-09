using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Queries.GetInventoryMovements;

public record InventoryMovementDto(
    Guid MovementId,
    string Type,
    int Quantity,
    string Reason,
    string UserName,
    DateTime Date);

public record GetInventoryMovementsQuery(Guid ProductId, Guid BranchId) : IRequest<List<InventoryMovementDto>>;

public class GetInventoryMovementsHandler : IRequestHandler<GetInventoryMovementsQuery, List<InventoryMovementDto>>
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetInventoryMovementsHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<List<InventoryMovementDto>> Handle(GetInventoryMovementsQuery request, CancellationToken cancellationToken)
    {
        var movements = await _inventoryRepository.GetMovementsAsync(request.ProductId, request.BranchId, cancellationToken);

        return movements.Select(m => new InventoryMovementDto(
            m.Id,
            m.Type == SaasDental.Domain.Enums.MovementType.Entry ? "Entrada" : "Salida",
            m.Quantity,
            m.Reason,
            m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Sistema",
            m.CreatedAt
        )).ToList();
    }
}
