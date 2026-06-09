using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 3 (CLÍNICO - DOMINIO): Actualización de datos médicos y filiación.
public class ClinicalHistoryTests
{
    // Verifica que los médicos puedan actualizar los signos vitales del paciente
    // (Exploración física básica) y que el sistema registre la fecha de modificación.
    [Fact]
    public void UpdatePhysicalExam_ConNuevosDatos_DebeActualizarCamposYFecha()
    {
        // Arrange
        var history = new ClinicalHistory(Guid.NewGuid());
        
        // Act
        history.UpdatePhysicalExam("120/80", "75 lpm", "36.5", "16 rpm", "Paciente aparentemente sano");

        // Assert
        history.BloodPressure.Should().Be("120/80");
        history.HeartRate.Should().Be("75 lpm");
        history.Temperature.Should().Be("36.5");
        history.UpdatedAt.Should().NotBeNull();
    }

    // Verifica la actualización de los motivos de consulta clínica.
    [Fact]
    public void UpdateIllnessAndHistory_ConNuevosDatos_DebeActualizarCamposYFecha()
    {
        // Arrange
        var history = new ClinicalHistory(Guid.NewGuid());
        
        // Act
        history.UpdateIllnessAndHistory("Dolor de muela", "Empezó hace 3 días tras comer", "Padre diabético", "Alergia a penicilina");

        // Assert
        history.CurrentIllnessReason.Should().Be("Dolor de muela");
        history.FamilyHistory.Should().Be("Padre diabético");
        history.UpdatedAt.Should().NotBeNull();
    }
}
