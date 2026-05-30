using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Tenants.Commands.CreateTenant;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Tenants.Commands;

// PRUEBA DEL MÓDULO 1 (ADMINISTRACIÓN Y ACCESOS): Validaciones de creación de clínicas (Tenants) y aislamiento de datos.
public class CreateTenantHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly CreateTenantHandler _handler;

    public CreateTenantHandlerTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _handler = new CreateTenantHandler(_tenantRepositoryMock.Object);
    }

    // Verifica que un SuperAdministrador pueda dar de alta a una nueva Clínica (Tenant)
    // y que esto asigne correctamente una base de aislamiento de datos en el sistema.
    [Fact]
    public async Task Handle_WithValidName_ShouldCreateTenantAndReturnResult()
    {
        // Arrange
        var command = new CreateTenantCommand("Clínica Dental Sonrisas", "Av. Principal 123");

        _tenantRepositoryMock.Setup(r => r.ExistsByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _tenantRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _tenantRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Clínica Dental Sonrisas");
        result.Address.Should().Be("Av. Principal 123");
        result.IsActive.Should().BeTrue();
        result.Id.Should().NotBeEmpty();

        _tenantRepositoryMock.Verify(r => r.AddAsync(It.Is<Tenant>(t => t.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
        _tenantRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // Valida que no se puedan crear dos clínicas con exactamente el mismo nombre,
    // evitando confusiones administrativas y posibles mezclas de subdominios.
    [Fact]
    public async Task Handle_WithExistingName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateTenantCommand("Clínica Dental Sonrisas", "Av. Principal 123");

        _tenantRepositoryMock.Setup(r => r.ExistsByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Simulate already exists

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Ya existe una clínica registrada con el nombre '{command.Name}'.");
            
        _tenantRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
