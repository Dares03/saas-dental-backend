using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Inventory.Queries.GetProductCategories;

public record ProductCategoryDto(Guid Id, string Name, string? Description, bool IsActive);

public record GetProductCategoriesQuery() : IRequest<List<ProductCategoryDto>>;

public class GetProductCategoriesHandler : IRequestHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITenantService _tenantService;

    public GetProductCategoriesHandler(IInventoryRepository inventoryRepository, ITenantService tenantService)
    {
        _inventoryRepository = inventoryRepository;
        _tenantService = tenantService;
    }

    public async Task<List<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var categories = await _inventoryRepository.GetProductCategoriesAsync(cancellationToken);

        // Auto-generate if empty
        if (!categories.Any())
        {
            var defaults = new List<ProductCategory>
            {
                new ProductCategory("Insumos Clínicos", "Materiales de uso general en consulta", tenantId),
                new ProductCategory("Medicamentos", "Fármacos, anestésicos y afines", tenantId),
                new ProductCategory("Instrumental", "Herramientas de uso odontológico", tenantId),
                new ProductCategory("Material Descartable", "Guantes, mascarillas, gasas, etc.", tenantId)
            };

            foreach (var cat in defaults)
            {
                await _inventoryRepository.AddProductCategoryAsync(cat, cancellationToken);
                categories.Add(cat);
            }
        }

        return categories
            .Select(c => new ProductCategoryDto(c.Id, c.Name, c.Description, c.IsActive))
            .ToList();
    }
}
