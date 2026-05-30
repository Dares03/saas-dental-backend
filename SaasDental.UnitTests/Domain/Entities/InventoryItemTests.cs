using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 5 (INVENTARIO - DOMINIO): Validaciones matemáticas del control de stock (Kardex puro).
public class InventoryItemTests
{
    // Verifica que cuando se registra un movimiento de Entrada, el stock actual (CurrentStock) 
    // se sume correctamente manteniendo la integridad matemática.
    [Fact]
    public void ApplyMovement_ConEntrada_DebeAumentarStock()
    {
        // Arrange
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        item.ApplyMovement(10, MovementType.Entry);
        item.ApplyMovement(5, MovementType.Entry);

        // Assert
        item.CurrentStock.Should().Be(15);
    }

    // Verifica que cuando se registra un movimiento de Salida, el stock actual se reste
    // de manera correcta reflejando la salida del insumo.
    [Fact]
    public void ApplyMovement_ConSalida_DebeDisminuirStock()
    {
        // Arrange
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        item.ApplyMovement(20, MovementType.Entry); // Saldo inicial: 20

        // Act
        item.ApplyMovement(5, MovementType.Exit);

        // Assert
        item.CurrentStock.Should().Be(15);
    }

    // Comprueba una regla de negocio crítica: No se puede despachar o sacar más insumos
    // de los que hay disponibles en el almacén. Debe lanzar un error InvalidOperationException.
    [Fact]
    public void ApplyMovement_ConSalidaMayorAlStock_DebeLanzarInvalidOperationException()
    {
        // Arrange
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        item.ApplyMovement(10, MovementType.Entry); // Saldo inicial: 10

        // Act
        Action act = () => item.ApplyMovement(15, MovementType.Exit); // Intenta sacar 15

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No hay suficiente stock para realizar esta salida.");
            
        // Validar que el stock se mantuvo intacto después del error
        item.CurrentStock.Should().Be(10);
    }

    // Asegura que nadie pueda alterar el inventario ingresando cantidades negativas o 0,
    // ya que matemáticamente rompería la lógica del kardex.
    [Fact]
    public void ApplyMovement_ConCantidadCeroONegativa_DebeLanzarArgumentException()
    {
        // Arrange
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        Action actZero = () => item.ApplyMovement(0, MovementType.Entry);
        Action actNegative = () => item.ApplyMovement(-5, MovementType.Entry);

        // Assert
        actZero.Should().Throw<ArgumentException>()
            .WithMessage("La cantidad del movimiento debe ser mayor a cero.");
            
        actNegative.Should().Throw<ArgumentException>()
            .WithMessage("La cantidad del movimiento debe ser mayor a cero.");
    }
}
