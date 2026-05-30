using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Clinical.Commands.AddFindingToOdontogram;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Features.Clinical.Commands;

// PRUEBA DEL MÓDULO 3 (CLÍNICO): Validaciones del guardado de hallazgos médicos en el Odontograma y creación dinámica de dientes.
public class AddFindingToOdontogramHandlerTests
{
    private readonly Mock<IClinicalRepository> _clinicalRepositoryMock;
    private readonly AddFindingToOdontogramHandler _handler;

    public AddFindingToOdontogramHandlerTests()
    {
        _clinicalRepositoryMock = new Mock<IClinicalRepository>();
        _handler = new AddFindingToOdontogramHandler(_clinicalRepositoryMock.Object);
    }

    // Verifica que si el diente y la cara ya existen en la base de datos, 
    // el hallazgo médico se añada exitosamente a esa cara existente.
    [Fact]
    public async Task Handle_WithExistingToothAndSurface_ShouldAddFindingToSurface()
    {
        // Arrange
        var odontogramId = Guid.NewGuid();
        var odontogram = new Odontogram(Guid.NewGuid(), OdontogramVersionType.Initial);
        
        // Force the Id of the odontogram to match our test
        typeof(Odontogram).GetProperty("Id")?.SetValue(odontogram, odontogramId);

        var tooth = new Tooth(18, odontogramId);
        typeof(Tooth).GetProperty("Id")?.SetValue(tooth, Guid.NewGuid());
        odontogram.Teeth.Add(tooth);

        var surface = new ToothSurface(SurfaceType.Vestibular, tooth.Id);
        typeof(ToothSurface).GetProperty("Id")?.SetValue(surface, Guid.NewGuid());
        tooth.Surfaces.Add(surface);

        _clinicalRepositoryMock.Setup(r => r.GetOdontogramByIdAsync(odontogramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(odontogram);

        var command = new AddFindingToOdontogramCommand(
            odontogramId,
            18,
            SurfaceType.Vestibular,
            "Caries",
            FindingColor.Red,
            "FDI"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _clinicalRepositoryMock.Verify(r => r.AddClinicalFindingAsync(It.Is<ClinicalFinding>(f => 
            f.FindingType == "Caries" &&
            f.Color == FindingColor.Red &&
            f.ToothId == tooth.Id &&
            f.ToothSurfaceId == surface.Id), It.IsAny<CancellationToken>()), Times.Once);
            
        _clinicalRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        // Ensure no new tooth or surface was duplicated
        odontogram.Teeth.Should().HaveCount(1);
        tooth.Surfaces.Should().HaveCount(1);
    }

    // Verifica la capacidad de "Creación Dinámica" del Odontograma: Si el doctor añade una 
    // caries a un diente que aún no estaba registrado en BD, el sistema debe crear
    // el Diente y la Cara al vuelo antes de guardar el hallazgo.
    [Fact]
    public async Task Handle_WithNonExistingTooth_ShouldCreateToothAndSurfaceAndAddFinding()
    {
        // Arrange
        var odontogramId = Guid.NewGuid();
        var odontogram = new Odontogram(Guid.NewGuid(), OdontogramVersionType.Initial);
        typeof(Odontogram).GetProperty("Id")?.SetValue(odontogram, odontogramId);

        _clinicalRepositoryMock.Setup(r => r.GetOdontogramByIdAsync(odontogramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(odontogram);

        var command = new AddFindingToOdontogramCommand(
            odontogramId,
            21,
            SurfaceType.Palatina,
            "Resina",
            FindingColor.Blue,
            "FDI"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        // Assert the tooth was created
        odontogram.Teeth.Should().HaveCount(1);
        var createdTooth = odontogram.Teeth.First();
        createdTooth.ToothNumber.Should().Be(21);

        // Assert the surface was created
        createdTooth.Surfaces.Should().HaveCount(1);
        createdTooth.Surfaces.First().SurfaceType.Should().Be(SurfaceType.Palatina);

        _clinicalRepositoryMock.Verify(r => r.AddClinicalFindingAsync(It.Is<ClinicalFinding>(f => 
            f.FindingType == "Resina" &&
            f.Color == FindingColor.Blue), It.IsAny<CancellationToken>()), Times.Once);
            
        _clinicalRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // Valida que el sistema lance un InvalidOperationException protegiendo los datos
    // en caso de que se intente añadir un hallazgo a un odontograma que no existe o es inválido.
    [Fact]
    public async Task Handle_WithInvalidOdontogram_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var odontogramId = Guid.NewGuid();

        _clinicalRepositoryMock.Setup(r => r.GetOdontogramByIdAsync(odontogramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Odontogram?)null); // Odontogram not found

        var command = new AddFindingToOdontogramCommand(
            odontogramId,
            18,
            SurfaceType.Vestibular,
            "Caries",
            FindingColor.Red,
            "FDI"
        );

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"No se encontró el odontograma con ID {odontogramId}.");
            
        _clinicalRepositoryMock.Verify(r => r.AddClinicalFindingAsync(It.IsAny<ClinicalFinding>(), It.IsAny<CancellationToken>()), Times.Never);
        _clinicalRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
