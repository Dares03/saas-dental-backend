using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Inventory.Commands.RegisterInventoryMovement;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Features.Inventory.Commands;

// PRUEBA DEL MÓDULO 5 (INVENTARIO): Validaciones de entradas/salidas de almacén y protección contra ventas sin stock.
public class RegisterInventoryMovementHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly RegisterInventoryMovementHandler _handler;

    public RegisterInventoryMovementHandlerTests()
    {
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new RegisterInventoryMovementHandler(
            _inventoryRepositoryMock.Object,
            _tenantServiceMock.Object
        );
    }

    // Comprueba que si se registra un movimiento de SALIDA de almacén y hay stock,
    // el sistema de Aplicación extraiga el item, aplique el movimiento y lo guarde.
    [Fact]
    public async Task Handle_WithExitMovement_ShouldReduceStock()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        var inventoryItem = new InventoryItem(productId, branchId, tenantId);
        // Add some initial stock
        inventoryItem.ApplyMovement(10, MovementType.Entry); 

        _inventoryRepositoryMock.Setup(r => r.GetInventoryItemAsync(productId, branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        var command = new RegisterInventoryMovementCommand(
            productId,
            branchId,
            MovementType.Exit,
            4,
            "Uso en consultorio 1"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        inventoryItem.CurrentStock.Should().Be(6); // 10 - 4 = 6

        _inventoryRepositoryMock.Verify(r => r.AddInventoryMovementAsync(It.Is<InventoryMovement>(m => 
            m.Quantity == 4 && m.Type == MovementType.Exit), It.IsAny<CancellationToken>()), Times.Once);
            
        _inventoryRepositoryMock.Verify(r => r.UpdateInventoryItemAsync(inventoryItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Valida la intercepción a nivel de Aplicación: Si se intenta hacer una salida
    // de un producto sin stock, el caso de uso se interrumpe y lanza InvalidOperationException.
    [Fact]
    public async Task Handle_WithExitMovementInsufficientStock_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        var inventoryItem = new InventoryItem(productId, branchId, tenantId);
        inventoryItem.ApplyMovement(2, MovementType.Entry); 

        _inventoryRepositoryMock.Setup(r => r.GetInventoryItemAsync(productId, branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        var command = new RegisterInventoryMovementCommand(
            productId,
            branchId,
            MovementType.Exit,
            5,
            "Uso excesivo"
        );

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No hay suficiente stock para realizar esta salida.");
            
        _inventoryRepositoryMock.Verify(r => r.AddInventoryMovementAsync(It.IsAny<InventoryMovement>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
