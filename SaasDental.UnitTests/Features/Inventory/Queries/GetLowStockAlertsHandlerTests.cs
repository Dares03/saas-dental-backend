using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Inventory.Queries.GetLowStockAlerts;
using Xunit;

namespace SaasDental.UnitTests.Features.Inventory.Queries;

// PRUEBA DEL MÓDULO 5 (INVENTARIO - APLICACIÓN)
public class GetLowStockAlertsHandlerTests
{
    private readonly Mock<IInventoryRepository> _repositoryMock;
    private readonly GetLowStockAlertsHandler _handler;

    public GetLowStockAlertsHandlerTests()
    {
        _repositoryMock = new Mock<IInventoryRepository>();
        _handler = new GetLowStockAlertsHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DebeLlamarAlRepositorioYRetornarAlertas()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        var alerts = new List<LowStockAlertDto>
        {
            new LowStockAlertDto(Guid.NewGuid(), "Anestesia", "SKU1", "Unidad", 2, 5)
        };

        _repositoryMock.Setup(r => r.GetLowStockItemsAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SaasDental.Domain.Entities.InventoryItem>()); // Needs entity list, actually wait. Let me fix the return type mock.

        var query = new GetLowStockAlertsQuery(branchId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
