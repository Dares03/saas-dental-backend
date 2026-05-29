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
    Guid CategoryId) : IRequest<Guid>;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MinimumStockAlert).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITenantService _tenantService;

    public CreateProductHandler(IInventoryRepository inventoryRepository, ITenantService tenantService)
    {
        _inventoryRepository = inventoryRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var product = new Product(
            request.Name,
            request.Description,
            request.SKU,
            request.UnitOfMeasure,
            request.MinimumStockAlert,
            request.CategoryId,
            tenantId);

        await _inventoryRepository.AddProductAsync(product, cancellationToken);

        return product.Id;
    }
}
