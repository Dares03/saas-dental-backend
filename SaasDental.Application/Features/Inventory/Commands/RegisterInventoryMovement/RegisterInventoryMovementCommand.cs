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
    private readonly ITenantRepository _tenantRepository;

    public RegisterInventoryMovementHandler(IInventoryRepository inventoryRepository, ITenantService tenantService, ITenantRepository tenantRepository)
    {
        _inventoryRepository = inventoryRepository;
        _tenantService = tenantService;
        _tenantRepository = tenantRepository;
    }

    public async Task<Guid> Handle(RegisterInventoryMovementCommand request, CancellationToken cancellationToken)
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
        
        var currentUserId = _tenantService.GetCurrentUserId() 
            ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

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
