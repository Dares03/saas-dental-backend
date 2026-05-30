using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Financial.Commands.AddIncome;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Features.Financial.Commands;

// PRUEBA DEL MÓDULO 4 (FINANCIERO): Validaciones de registro de ingresos y cálculo matemático de saldos de caja.
public class AddIncomeHandlerTests
{
    private readonly Mock<IFinancialRepository> _financialRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly AddIncomeHandler _handler;

    public AddIncomeHandlerTests()
    {
        _financialRepositoryMock = new Mock<IFinancialRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new AddIncomeHandler(
            _financialRepositoryMock.Object,
            _tenantServiceMock.Object
        );
    }

    // Valida que si la caja registradora está legalmente ABIERTA, la capa de aplicación
    // guarde exitosamente la transacción y devuelva su ID para imprimir el recibo.
    [Fact]
    public async Task Handle_WithOpenRegister_ShouldAddIncomeAndUpdateBalance()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var registerId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        var openRegister = new CashRegister(100m, Guid.NewGuid(), Guid.NewGuid(), tenantId);
        typeof(CashRegister).GetProperty("Id")?.SetValue(openRegister, registerId);

        _financialRepositoryMock.Setup(r => r.GetCashRegisterByIdAsync(registerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(openRegister);

        var command = new AddIncomeCommand(
            registerId,
            50m,
            "Pago de consulta",
            PaymentMethod.Cash,
            null
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        openRegister.CalculatedFinalBalance.Should().Be(150m);

        _financialRepositoryMock.Verify(r => r.AddCashTransactionAsync(It.Is<CashTransaction>(t => 
            t.Amount == 50m &&
            t.Type == TransactionType.Income &&
            t.CashRegisterId == registerId), It.IsAny<CancellationToken>()), Times.Once);
            
        _financialRepositoryMock.Verify(r => r.UpdateCashRegisterAsync(openRegister, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Protege el flujo de la clínica: Si un cajero intenta cobrar a un paciente
    // pero la caja está CERRADA, se debe rechazar la operación a nivel de Aplicación.
    [Fact]
    public async Task Handle_WithClosedRegister_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var registerId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        var closedRegister = new CashRegister(100m, Guid.NewGuid(), Guid.NewGuid(), tenantId);
        closedRegister.Close(100m); // Close it
        
        _financialRepositoryMock.Setup(r => r.GetCashRegisterByIdAsync(registerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(closedRegister);

        var command = new AddIncomeCommand(
            registerId,
            50m,
            "Pago de consulta",
            PaymentMethod.Cash,
            null
        );

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La caja seleccionada está cerrada.");
            
        _financialRepositoryMock.Verify(r => r.AddCashTransactionAsync(It.IsAny<CashTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
