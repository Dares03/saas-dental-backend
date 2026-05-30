using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 2 (CRM Y AGENDA - DOMINIO): Validaciones del comportamiento puro del Paciente.
public class PatientTests
{
    // Verifica que al instanciar un nuevo paciente, por defecto quede en estado Activo
    // para que pueda ser listado inmediatamente en el CRM.
    [Fact]
    public void Constructor_ConNuevosDatos_DebeInicializarComoActivo()
    {
        // Arrange & Act
        var patient = new Patient("Juan", "Perez", "71234567", new DateTime(1990, 1, 1), "999888777", "juan@test.com", Guid.NewGuid());

        // Assert
        patient.IsActive.Should().BeTrue();
        patient.FirstName.Should().Be("Juan");
    }

    // Valida que al desactivar a un paciente (ej. Archivo muerto), el estado interno cambie
    // y se registre el sello de tiempo (UpdatedAt).
    [Fact]
    public void ChangeStatus_AInactivo_DebeActualizarEstadoYFecha()
    {
        // Arrange
        var patient = new Patient("Juan", "Perez", "71234567", new DateTime(1990, 1, 1), "999888777", "juan@test.com", Guid.NewGuid());
        patient.IsActive.Should().BeTrue();

        // Act
        patient.ChangeStatus(false);

        // Assert
        patient.IsActive.Should().BeFalse();
        patient.UpdatedAt.Should().NotBeNull();
    }
}
