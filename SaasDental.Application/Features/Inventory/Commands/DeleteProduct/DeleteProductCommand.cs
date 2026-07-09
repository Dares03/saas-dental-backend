using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Commands.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : IRequest<bool>;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IInventoryRepository _inventoryRepository;

    public DeleteProductHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _inventoryRepository.GetProductByIdAsync(request.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Producto con ID '{request.ProductId}' no encontrado.");

        var hasMovements = await _inventoryRepository.HasInventoryMovementsAsync(request.ProductId, cancellationToken);

        if (hasMovements)
        {
            // Soft delete because it has historical movements
            product.Deactivate();
            await _inventoryRepository.UpdateProductAsync(product, cancellationToken);
        }
        else
        {
            // Hard delete because it has no historical data
            await _inventoryRepository.DeleteProductAsync(product, cancellationToken);
        }

        return true;
    }
}
