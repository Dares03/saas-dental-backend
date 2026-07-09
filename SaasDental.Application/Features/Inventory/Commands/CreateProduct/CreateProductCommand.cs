using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Inventory.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    string? SKU,
    string UnitOfMeasure,
    int MinimumStockAlert,
    Guid CategoryId,
    Guid BranchId) : IRequest<Guid>;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MinimumStockAlert).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITenantService _tenantService;
    private readonly ITenantRepository _tenantRepository;

    public CreateProductHandler(IInventoryRepository inventoryRepository, ITenantService tenantService, ITenantRepository tenantRepository)
    {
        _inventoryRepository = inventoryRepository;
        _tenantService = tenantService;
        _tenantRepository = tenantRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        // Auto-heal: Ensure Branch exists
        var branch = await _tenantRepository.GetBranchByIdAsync(request.BranchId, cancellationToken);
        if (branch == null)
        {
            branch = new Branch("Sede Principal", "Dirección no especificada", "", tenantId);
            branch.SetId(request.BranchId);
            await _tenantRepository.AddBranchAsync(branch, cancellationToken);
            await _tenantRepository.SaveChangesAsync(cancellationToken);
        }

        var product = new Product(
            request.Name,
            request.Description,
            request.SKU,
            request.UnitOfMeasure,
            request.MinimumStockAlert,
            request.CategoryId,
            tenantId);

        await _inventoryRepository.AddProductAsync(product, cancellationToken);

        // Crear automáticamente el InventoryItem para la sede actual
        var inventoryItem = new InventoryItem(product.Id, request.BranchId, tenantId);
        await _inventoryRepository.AddInventoryItemAsync(inventoryItem, cancellationToken);

        return product.Id;
    }
}

