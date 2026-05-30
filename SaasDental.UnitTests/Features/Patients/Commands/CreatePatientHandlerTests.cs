using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Patients.Commands.CreatePatient;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Patients.Commands;

// PRUEBA DEL MÓDULO 2 (CRM Y AGENDA): Validaciones de creación de pacientes, historial clínico automático y protección contra DNIs duplicados.
public class CreatePatientHandlerTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IClinicalRepository> _clinicalRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly CreatePatientHandler _handler;

    public CreatePatientHandlerTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _clinicalRepositoryMock = new Mock<IClinicalRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new CreatePatientHandler(
            _patientRepositoryMock.Object,
            _clinicalRepositoryMock.Object,
            _tenantServiceMock.Object
        );
    }

    // Verifica que el proceso de alta de un paciente cree no solo sus datos personales
    // sino que también genere automáticamente su primera Historia Clínica ligada.
    [Fact]
    public async Task Handle_WithValidData_ShouldCreatePatientAndClinicalHistory()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Patient>());

        var command = new CreatePatientCommand(
            "Juan", "Pérez", "12345678", new DateTime(1990, 1, 1), "999888777", "juan@test.com", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _patientRepositoryMock.Verify(r => r.AddAsync(It.Is<Patient>(p => 
            p.FirstName == "Juan" && 
            p.DocumentId == "12345678" &&
            p.TenantId == tenantId), It.IsAny<CancellationToken>()), Times.Once);

        _clinicalRepositoryMock.Verify(r => r.AddHistoryAsync(It.Is<ClinicalHistory>(h => 
            h.PatientId == result), It.IsAny<CancellationToken>()), Times.Once);
    }

    // Protege contra la duplicidad de pacientes. Si la clínica intenta registrar
    // a alguien con un DNI o Pasaporte que ya existe, lanza un InvalidOperationException.
    [Fact]
    public async Task Handle_WithDuplicateDocumentId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantServiceMock.Setup(t => t.GetCurrentTenantId()).Returns(tenantId);
        
        var existingPatient = new Patient("Maria", "Gomez", "12345678", null, null, null, tenantId);
        
        _patientRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Patient> { existingPatient });

        var command = new CreatePatientCommand(
            "Juan", "Pérez", "12345678", new DateTime(1990, 1, 1), "999888777", "juan@test.com", null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ya existe un paciente con el documento '12345678'.");
            
        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
        _clinicalRepositoryMock.Verify(r => r.AddHistoryAsync(It.IsAny<ClinicalHistory>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
