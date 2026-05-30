using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 2 (CRM Y AGENDA - DOMINIO): Validaciones del comportamiento puro de una Cita.
public class AppointmentTests
{
    // Verifica que al agendar una nueva cita en el calendario, esta nazca automáticamente
    // bajo el estado de "Programada" (Scheduled), lista para ser atendida o cancelada luego.
    [Fact]
    public void Constructor_ConNuevosDatos_DebeInicializarComoProgramada()
    {
        // Arrange & Act
        var appointment = new Appointment(DateTime.UtcNow.AddDays(1), 30, "Consulta General", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Scheduled);
        appointment.DurationMinutes.Should().Be(30);
    }

    // Valida que el flujo de vida de una cita funcione: Si el doctor marca la cita 
    // como "Completada", el estado debe reflejarse correctamente.
    [Fact]
    public void ChangeStatus_ACompletado_DebeActualizarEstadoYFecha()
    {
        // Arrange
        var appointment = new Appointment(DateTime.UtcNow.AddDays(1), 30, "Consulta General", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        appointment.ChangeStatus(AppointmentStatus.Completed);

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Completed);
        appointment.UpdatedAt.Should().NotBeNull();
    }
}
