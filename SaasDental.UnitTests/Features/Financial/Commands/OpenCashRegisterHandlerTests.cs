using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Financial.Commands.OpenCashRegister;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Financial.Commands;

// PRUEBA DEL MÓDULO 4 (FINANCIERO): Validaciones de apertura de caja y bloqueos de doble apertura.
public class OpenCashRegisterHandlerTests
{
    private readonly Mock<IFinancialRepository> _financialRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly OpenCashRegisterHandler _handler;

    public OpenCashRegisterHandlerTests()
    {
        _financialRepositoryMock = new Mock<IFinancialRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new OpenCashRegisterHandler(
            _financialRepositoryMock.Object,
            _tenantServiceMock.Object
        );
    }

    // Comprueba que si la sede de la clínica no tiene una caja activa en este momento,
    // el sistema permite abrir un nuevo turno/caja registradora exitosamente.
    [Fact]
    public async Task Handle_WithValidData_ShouldOpenCashRegister()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        _financialRepositoryMock.Setup(r => r.GetActiveCashRegisterAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CashRegister?)null);

        var command = new OpenCashRegisterCommand(branchId, 100.50m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _financialRepositoryMock.Verify(r => r.AddCashRegisterAsync(It.Is<CashRegister>(cr => 
            cr.InitialBalance == 100.50m &&
            cr.BranchId == branchId &&
            cr.TenantId == tenantId &&
            cr.Status == SaasDental.Domain.Enums.CashRegisterStatus.Open), It.IsAny<CancellationToken>()), Times.Once);
    }

    // Verifica la regla de prevención de doble apertura: Una sede no puede tener
    // dos cajas abiertas simultáneamente. Si se intenta, lanza InvalidOperationException.
    [Fact]
    public async Task Handle_WithExistingOpenRegister_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        var openRegister = new CashRegister(50m, branchId, Guid.NewGuid(), tenantId);

        _financialRepositoryMock.Setup(r => r.GetActiveCashRegisterAsync(branchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(openRegister);

        var command = new OpenCashRegisterCommand(branchId, 100m);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ya existe una caja abierta para esta sede.");
            
        _financialRepositoryMock.Verify(r => r.AddCashRegisterAsync(It.IsAny<CashRegister>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
