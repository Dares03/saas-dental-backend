using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 4 (FINANCIERO - DOMINIO): Lógica de Deudas y Pagos de Pacientes.
public class PatientDebtTests
{
    // Verifica que cuando se registra un pago parcial, el estado cambie a "Parcial"
    // y el monto restante se calcule matemáticamente correcto.
    [Fact]
    public void AddPayment_PagoParcial_DebeCambiarEstadoAParcialYCalcularRestante()
    {
        // Arrange
        var debt = new PatientDebt(100, "Ortodoncia Cuota 1", Guid.NewGuid(), null, Guid.NewGuid());

        // Act
        debt.AddPayment(40);

        // Assert
        debt.Status.Should().Be(DebtStatus.Partial);
        debt.PaidAmount.Should().Be(40);
        debt.RemainingAmount.Should().Be(60);
    }

    // Verifica que cuando el paciente abona el 100% de la deuda o más, 
    // el sistema marque la deuda como "Pagada" de manera automática.
    [Fact]
    public void AddPayment_PagoTotal_DebeCambiarEstadoAPagado()
    {
        // Arrange
        var debt = new PatientDebt(100, "Limpieza", Guid.NewGuid(), null, Guid.NewGuid());

        // Act
        debt.AddPayment(100);

        // Assert
        debt.Status.Should().Be(DebtStatus.Paid);
        debt.PaidAmount.Should().Be(100);
        debt.RemainingAmount.Should().Be(0);
    }

    // Asegura que no se puedan inyectar pagos negativos o ceros, 
    // previniendo alteraciones ilegales a las cuentas por cobrar.
    [Fact]
    public void AddPayment_PagoInvalido_DebeLanzarArgumentException()
    {
        // Arrange
        var debt = new PatientDebt(100, "Limpieza", Guid.NewGuid(), null, Guid.NewGuid());

        // Act
        Action actZero = () => debt.AddPayment(0);
        Action actNegative = () => debt.AddPayment(-50);

        // Assert
        actZero.Should().Throw<ArgumentException>().WithMessage("El monto debe ser mayor a cero.");
        actNegative.Should().Throw<ArgumentException>().WithMessage("El monto debe ser mayor a cero.");
    }

    // Simula el escenario en el que se revierte un pago por error de caja.
    // La deuda que antes estaba "Pagada" o "Parcial" debería regresar al estado "Pendiente".
    [Fact]
    public void RemovePayment_RevertirPago_DebeRegresarEstadoAPendiente()
    {
        // Arrange
        var debt = new PatientDebt(100, "Extracción", Guid.NewGuid(), null, Guid.NewGuid());
        debt.AddPayment(40); // Queda en 40 pagado (Partial)

        // Act
        debt.RemovePayment(40); // Revertimos el pago

        // Assert
        debt.Status.Should().Be(DebtStatus.Pending);
        debt.PaidAmount.Should().Be(0);
        debt.RemainingAmount.Should().Be(100);
    }
}
