using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Inventory.Commands.CreateProductCategory;

public record CreateProductCategoryCommand(string Name, string? Description) : IRequest<Guid>;

public class CreateProductCategoryValidator : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class CreateProductCategoryHandler : IRequestHandler<CreateProductCategoryCommand, Guid>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITenantService _tenantService;

    public CreateProductCategoryHandler(IInventoryRepository inventoryRepository, ITenantService tenantService)
    {
        _inventoryRepository = inventoryRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var category = new ProductCategory(request.Name, request.Description, tenantId);

        await _inventoryRepository.AddProductCategoryAsync(category, cancellationToken);

        return category.Id;
    }
}
