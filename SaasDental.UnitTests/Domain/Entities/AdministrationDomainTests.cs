using System;
using FluentAssertions;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Domain.Entities;

// PRUEBA DEL MÓDULO 1 (TENANTS Y SEGURIDAD - DOMINIO): Lógica pura de administración.
public class AdministrationDomainTests
{
    // Verifica que al crear un Tenant (Clínica), este inicie activo y se puedan actualizar sus datos.
    [Fact]
    public void Tenant_UpdateDetails_DebeActualizarCamposYFecha()
    {
        // Arrange
        var tenant = new Tenant("Clínica Centro", "Av. Principal 123");
        tenant.IsActive.Should().BeTrue();

        // Act
        tenant.UpdateDetails("Clínica Centro Actualizada", "Av. Secundaria 456");

        // Assert
        tenant.Name.Should().Be("Clínica Centro Actualizada");
        tenant.Address.Should().Be("Av. Secundaria 456");
        tenant.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Tenant_Deactivate_DebeCambiarEstadoAInactivo()
    {
        // Arrange
        var tenant = new Tenant("Clínica Sur", "Calle 1");

        // Act
        tenant.Deactivate();

        // Assert
        tenant.IsActive.Should().BeFalse();
    }

    // Verifica la lógica de asignación de Sede por defecto para un usuario (Hot-switching).
    [Fact]
    public void User_SetDefaultBranch_DebeActualizarSedeYFecha()
    {
        // Arrange
        var user = new User("Admin", "Root", "admin@test.com", "hash", "Admin", Guid.NewGuid());
        var branchId = Guid.NewGuid();

        // Act
        user.SetDefaultBranch(branchId);

        // Assert
        user.DefaultBranchId.Should().Be(branchId);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void User_Deactivate_DebeDesactivarUsuario()
    {
        // Arrange
        var user = new User("Medico", "Uno", "medico@test.com", "hash", "Dentist", Guid.NewGuid());

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Branch_Deactivate_DebeDesactivarSede()
    {
        // Arrange
        var branch = new Branch("Sede Norte", "Av. Norte", "123456", Guid.NewGuid());

        // Act
        branch.Deactivate();

        // Assert
        branch.IsActive.Should().BeFalse();
    }
}
