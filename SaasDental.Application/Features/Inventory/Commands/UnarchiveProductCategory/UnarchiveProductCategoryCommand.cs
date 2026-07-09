using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Commands.UnarchiveProductCategory;

public record UnarchiveProductCategoryCommand(Guid CategoryId) : IRequest<bool>;

public class UnarchiveProductCategoryHandler : IRequestHandler<UnarchiveProductCategoryCommand, bool>
{
    private readonly IInventoryRepository _inventoryRepository;

    public UnarchiveProductCategoryHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(UnarchiveProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _inventoryRepository.GetProductCategoryByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoría con ID '{request.CategoryId}' no encontrada.");

        category.Activate();
        
        await _inventoryRepository.UpdateProductCategoryAsync(category, cancellationToken);
        return true;
    }
}
