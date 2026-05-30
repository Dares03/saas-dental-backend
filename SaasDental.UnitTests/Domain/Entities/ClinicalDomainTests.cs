using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 3 (CLÍNICO - DOMINIO): Validaciones de las reglas puras del Odontograma y Hallazgos.
public class ClinicalDomainTests
{
    // Verifica que un hallazgo clínico (ej. Caries en Rojo) pueda cambiar de color a Azul.
    // En odontología, cambiar de Rojo (Enfermedad) a Azul (Tratado) significa que el tratamiento se curó.
    [Fact]
    public void UpdateColor_DeRojoAAzul_DebeActualizarElEstadoDelTratamiento()
    {
        // Arrange
        var finding = new ClinicalFinding("Caries", FindingColor.Red, "O", Guid.NewGuid());
        
        // Act
        finding.UpdateColor(FindingColor.Blue);

        // Assert
        finding.Color.Should().Be(FindingColor.Blue);
        finding.UpdatedAt.Should().NotBeNull();
    }

    // Verifica que se puedan actualizar los campos de texto requeridos por las normas del MINSA
    // (Especificaciones y Observaciones) en el documento del odontograma.
    [Fact]
    public void UpdateTextFields_ConNuevosTextos_DebeActualizarCamposYFecha()
    {
        // Arrange
        var odontogram = new Odontogram(Guid.NewGuid(), OdontogramVersionType.Initial);
        
        // Act
        odontogram.UpdateTextFields("Múltiples caries", "Paciente no coopera mucho");

        // Assert
        odontogram.Specifications.Should().Be("Múltiples caries");
        odontogram.Observations.Should().Be("Paciente no coopera mucho");
        odontogram.UpdatedAt.Should().NotBeNull();
    }

    // Asegura que al crear un hallazgo a nivel de superficie de diente, 
    // se vinculen tanto el ID del diente como el ID de la cara (Superficie).
    [Fact]
    public void CrearFinding_NivelSuperficie_DebeAsignarAmbosIds()
    {
        // Arrange
        var toothId = Guid.NewGuid();
        var surfaceId = Guid.NewGuid();

        // Act
        var finding = new ClinicalFinding("Restauracion", FindingColor.Blue, "O", toothId, surfaceId);

        // Assert
        finding.ToothId.Should().Be(toothId);
        finding.ToothSurfaceId.Should().Be(surfaceId);
        finding.FindingType.Should().Be("Restauracion");
    }
}
