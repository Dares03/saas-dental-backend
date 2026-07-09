using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Inventory.Commands.CreateProduct;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Inventory.Commands;

// PRUEBA DEL MÓDULO 5 (INVENTARIO): Validaciones de creación de productos (insumos médicos).
public class CreateProductHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new CreateProductHandler(
            _inventoryRepositoryMock.Object,
            _tenantServiceMock.Object
        );
    }

    // Comprueba que los administradores puedan crear exitosamente nuevos insumos 
    // en la clínica (ej. "Anestesia Local") y se guarde en la BD.
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);

        var categoryId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Anestesia Local",
            "Caja de 50 cartuchos",
            "ANES-001",
            "Caja",
            5,
            categoryId,
            branchId
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _inventoryRepositoryMock.Verify(r => r.AddProductAsync(It.Is<Product>(p => 
            p.Name == "Anestesia Local" &&
            p.CategoryId == categoryId &&
            p.TenantId == tenantId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
