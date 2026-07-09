using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Inventory.Queries.GetProductCategories;

public record ProductCategoryDto(Guid Id, string Name, string? Description);

public record GetProductCategoriesQuery() : IRequest<List<ProductCategoryDto>>;

public class GetProductCategoriesHandler : IRequestHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetProductCategoriesHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<List<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _inventoryRepository.GetProductCategoriesAsync(cancellationToken);

        return categories.Select(c => new ProductCategoryDto(
            c.Id,
            c.Name,
            c.Description
        )).ToList();
    }
}
