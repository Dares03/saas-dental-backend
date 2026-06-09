using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBAS DE LOS MÓDULOS DE CATÁLOGOS (Productos, Servicios, Familiares)
public class CatalogDomainTests
{
    [Fact]
    public void Product_Deactivate_DebeDesactivarProducto()
    {
        // Arrange
        var product = new Product("Anestesia", "Tubo", "SKU1", "Unidad", 10, Guid.NewGuid(), Guid.NewGuid());
        product.IsActive.Should().BeTrue();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Product_UpdateDetails_DebeActualizarCampos()
    {
        // Arrange
        var product = new Product("Anestesia", "Tubo", "SKU1", "Unidad", 10, Guid.NewGuid(), Guid.NewGuid());

        // Act
        product.UpdateDetails("Anestesia Local", "Caja x 50", "SKU2", "Caja", 5, Guid.NewGuid());

        // Assert
        product.Name.Should().Be("Anestesia Local");
        product.SKU.Should().Be("SKU2");
    }

    [Fact]
    public void ProductCategory_UpdateDetails_DebeActualizarCampos()
    {
        // Arrange
        var category = new ProductCategory("Insumos", "Insumos generales", Guid.NewGuid());

        // Act
        category.UpdateDetails("Insumos Clínicos", "Insumos médicos");

        // Assert
        category.Name.Should().Be("Insumos Clínicos");
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ServiceCategory_UpdateDetails_DebeActualizarCampos()
    {
        // Arrange
        var category = new ServiceCategory("Ortodoncia", "Brackets", Guid.NewGuid());

        // Act
        category.UpdateDetails("Ortodoncia General", "Tratamientos");

        // Assert
        category.Name.Should().Be("Ortodoncia General");
    }

    [Fact]
    public void TreatmentService_Deactivate_DebeDesactivarTratamiento()
    {
        // Arrange
        var service = new TreatmentService("Curación Simple", null, 100, 40, Guid.NewGuid(), Guid.NewGuid());
        service.IsActive.Should().BeTrue();

        // Act
        service.Deactivate();

        // Assert
        service.IsActive.Should().BeFalse();
    }

    [Fact]
    public void TreatmentService_UpdateDetails_DebeActualizarPrecio()
    {
        // Arrange
        var service = new TreatmentService("Curación Simple", null, 100, 40, Guid.NewGuid(), Guid.NewGuid());

        // Act
        service.UpdateDetails("Curación Simple", "Diente", 150, 50, Guid.NewGuid());

        // Assert
        service.BasePrice.Should().Be(150);
        service.DoctorCommissionPercentage.Should().Be(50);
    }

    [Fact]
    public void PatientRelative_UpdateDetails_DebeActualizarFamiliar()
    {
        // Arrange
        var relative = new PatientRelative("Maria Perez", "Madre", "999", true, Guid.NewGuid());

        // Act
        relative.UpdateDetails("Maria Garcia", "Madre", "888", false);

        // Assert
        relative.FullName.Should().Be("Maria Garcia");
        relative.IsEmergencyContact.Should().BeFalse();
    }
}
