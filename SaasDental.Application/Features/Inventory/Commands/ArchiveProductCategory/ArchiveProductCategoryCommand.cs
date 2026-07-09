using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Commands.ArchiveProductCategory;

public record ArchiveProductCategoryCommand(Guid CategoryId) : IRequest<bool>;

public class ArchiveProductCategoryHandler : IRequestHandler<ArchiveProductCategoryCommand, bool>
{
    private readonly IInventoryRepository _inventoryRepository;

    public ArchiveProductCategoryHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(ArchiveProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _inventoryRepository.GetProductCategoryByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoría con ID '{request.CategoryId}' no encontrada.");

        category.Deactivate();
        await _inventoryRepository.UpdateProductCategoryAsync(category, cancellationToken);

        return true;
    }
}
