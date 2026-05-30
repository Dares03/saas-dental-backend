using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 4 (FINANCIERO - DOMINIO): Validaciones matemáticas y lógicas puras de una Caja Registradora.
public class CashRegisterTests
{
    // Verifica que cualquier transacción etiquetada como INGRESO se sume matemáticamente
    // al balance calculado de la caja, garantizando la integridad de los fondos.
    [Fact]
    public void AddTransaction_Ingreso_DebeSumarAlBalanceCalculado()
    {
        // Arrange
        var register = new CashRegister(100, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()); // Inicia con 100

        // Act
        register.AddTransaction(50.5m, TransactionType.Income);

        // Assert
        register.CalculatedFinalBalance.Should().Be(150.5m);
    }

    // Verifica que cualquier transacción etiquetada como EGRESO se reste matemáticamente
    // del balance calculado de la caja.
    [Fact]
    public void AddTransaction_Egreso_DebeRestarDelBalanceCalculado()
    {
        // Arrange
        var register = new CashRegister(200, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()); // Inicia con 200

        // Act
        register.AddTransaction(40, TransactionType.Expense);

        // Assert
        register.CalculatedFinalBalance.Should().Be(160);
    }

    // Comprueba una regla de seguridad crítica: Nadie puede registrar ni restar dinero
    // (Añadir Transacciones) a una caja que ya fue reportada como "Cerrada" por un administrador.
    [Fact]
    public void AddTransaction_EnCajaCerrada_DebeLanzarInvalidOperationException()
    {
        // Arrange
        var register = new CashRegister(100, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        register.Close(100); // Cerramos la caja

        // Act
        Action act = () => register.AddTransaction(50, TransactionType.Income);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No se pueden registrar transacciones en una caja cerrada.");
    }

    // Valida que el sistema rechace un doble cierre (cerrar una caja que ya está cerrada),
    // lo cual prevendría la sobrescritura de reportes financieros previos.
    [Fact]
    public void Close_CajaYaCerrada_DebeLanzarInvalidOperationException()
    {
        // Arrange
        var register = new CashRegister(100, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        register.Close(100); // Primer cierre exitoso

        // Act
        Action act = () => register.Close(100); // Intento de segundo cierre

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("La caja ya se encuentra cerrada.");
    }

    // Valida el flujo correcto del Cierre de Caja: El status cambia a "Closed",
    // se sella la fecha de cierre (ClosedAt) y se archiva el monto reportado físicamente.
    [Fact]
    public void Close_DebeCambiarEstadoYAsignarFecha()
    {
        // Arrange
        var register = new CashRegister(100, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        register.Status.Should().Be(CashRegisterStatus.Open);
        register.ClosedAt.Should().BeNull();

        // Act
        register.Close(150); // Reporte físico final 150

        // Assert
        register.Status.Should().Be(CashRegisterStatus.Closed);
        register.ClosedAt.Should().NotBeNull();
        register.ReportedFinalBalance.Should().Be(150);
    }
}
