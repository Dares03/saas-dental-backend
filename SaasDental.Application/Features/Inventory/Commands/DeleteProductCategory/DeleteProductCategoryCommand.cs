using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Commands.DeleteProductCategory;

public record DeleteProductCategoryCommand(Guid CategoryId) : IRequest<bool>;

public class DeleteProductCategoryHandler : IRequestHandler<DeleteProductCategoryCommand, bool>
{
    private readonly IInventoryRepository _inventoryRepository;

    public DeleteProductCategoryHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _inventoryRepository.GetProductCategoryByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoría con ID '{request.CategoryId}' no encontrada.");

        if (category.Products.Any())
        {
            throw new InvalidOperationException("No se puede eliminar esta categoría porque está siendo utilizada por uno o más productos. Por favor archívela o elimine los productos asociados primero.");
        }

        await _inventoryRepository.DeleteProductCategoryAsync(category, cancellationToken);
        return true;
    }
}
