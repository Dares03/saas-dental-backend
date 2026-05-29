using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Inventory.Commands.RegisterInventoryMovement;

public record RegisterInventoryMovementCommand(
    Guid ProductId,
    Guid BranchId,
    MovementType Type,
    int Quantity,
    string Reason) : IRequest<Guid>;

public class RegisterInventoryMovementValidator : AbstractValidator<RegisterInventoryMovementCommand>
{
    public RegisterInventoryMovementValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(255);
    }
}

public class RegisterInventoryMovementHandler : IRequestHandler<RegisterInventoryMovementCommand, Guid>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITenantService _tenantService;

    public RegisterInventoryMovementHandler(IInventoryRepository inventoryRepository, ITenantService tenantService)
    {
        _inventoryRepository = inventoryRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(RegisterInventoryMovementCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        // Verificar si existe el InventoryItem para ese Producto en esa Sede
        var inventoryItem = await _inventoryRepository.GetInventoryItemAsync(request.ProductId, request.BranchId, cancellationToken);
        
        if (inventoryItem == null)
        {
            if (request.Type == MovementType.Exit)
            {
                throw new InvalidOperationException("No se puede registrar una salida si el producto no tiene stock registrado en esta sede.");
            }

            // Si es entrada y no existe, lo creamos
            inventoryItem = new InventoryItem(request.ProductId, request.BranchId, tenantId);
            await _inventoryRepository.AddInventoryItemAsync(inventoryItem, cancellationToken);
        }

        // Aplicar el movimiento lógico al Stock Actual
        inventoryItem.ApplyMovement(request.Quantity, request.Type);
        
        // Mock User Id for now, since we don't have user injection yet
        var currentUserId = Guid.Empty;

        // Registrar el movimiento en el Kardex
        var movement = new InventoryMovement(
            request.Type,
            request.Quantity,
            request.Reason,
            inventoryItem.Id,
            currentUserId,
            tenantId);

        await _inventoryRepository.AddInventoryMovementAsync(movement, cancellationToken);
        await _inventoryRepository.UpdateInventoryItemAsync(inventoryItem, cancellationToken);

        return movement.Id;
    }
}
