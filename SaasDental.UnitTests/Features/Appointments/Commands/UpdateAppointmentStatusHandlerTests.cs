using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Appointments.Commands.UpdateAppointmentStatus;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Features.Appointments.Commands;

// PRUEBA DEL MÓDULO 2 (CRM Y AGENDA - APLICACIÓN)
public class UpdateAppointmentStatusHandlerTests
{
    private readonly Mock<IAppointmentRepository> _repositoryMock;
    private readonly UpdateAppointmentStatusHandler _handler;

    public UpdateAppointmentStatusHandlerTests()
    {
        _repositoryMock = new Mock<IAppointmentRepository>();
        _handler = new UpdateAppointmentStatusHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConDatosValidos_DebeActualizarEstadoGuardar()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();

        // Simulamos la cita existente en la base de datos
        var existingAppointment = new Appointment(DateTime.UtcNow, 30, "Chequeo", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), tenantId);
        _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAppointment);

        var command = new UpdateAppointmentStatusCommand(appointmentId, AppointmentStatus.Completed);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingAppointment.Status.Should().Be(AppointmentStatus.Completed);
        _repositoryMock.Verify(r => r.UpdateAsync(existingAppointment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConCitaInexistente_DebeLanzarException()
    {
        // Arrange
        // Simular que no se encontró en la BD
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment)null!);

        var command = new UpdateAppointmentStatusCommand(Guid.NewGuid(), AppointmentStatus.Cancelled);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Cita no encontrada.");
    }
}
