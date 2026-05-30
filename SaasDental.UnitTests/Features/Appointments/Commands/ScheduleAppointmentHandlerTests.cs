using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Appointments.Commands.ScheduleAppointment;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Appointments.Commands;

// PRUEBA DEL MÓDULO 2 (CRM Y AGENDA): Validaciones del agendamiento de citas en el calendario.
public class ScheduleAppointmentHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly ScheduleAppointmentHandler _handler;

    public ScheduleAppointmentHandlerTests()
    {
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new ScheduleAppointmentHandler(
            _appointmentRepositoryMock.Object,
            _tenantServiceMock.Object
        );
    }

    // Verifica que si los datos de la cita son válidos, se interactúe con el repositorio
    // para guardarla en base de datos y se retorne un identificador válido.
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateAppointmentAndReturnId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);

        var patientId = Guid.NewGuid();
        var dentistId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1);
        
        var command = new ScheduleAppointmentCommand(
            patientId, dentistId, branchId, date, 30, "Consulta de control"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _appointmentRepositoryMock.Verify(r => r.AddAsync(It.Is<Appointment>(a => 
            a.PatientId == patientId &&
            a.DentistId == dentistId &&
            a.BranchId == branchId &&
            a.DurationMinutes == 30 &&
            a.Reason == "Consulta de control" &&
            a.TenantId == tenantId), It.IsAny<CancellationToken>()), Times.Once);
    }

    // Verifica una regla de seguridad multi-tenant: Si no existe un tenant activo
    // en la sesión, la capa de aplicación debe rechazar la creación de la cita.
    [Fact]
    public async Task Handle_WithoutActiveTenant_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns((Guid?)null);

        var command = new ScheduleAppointmentCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 30, "Consulta"
        );

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("El contexto no tiene un Tenant válido.");
            
        _appointmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
