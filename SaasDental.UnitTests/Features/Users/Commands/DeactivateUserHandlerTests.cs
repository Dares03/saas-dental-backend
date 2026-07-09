using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Features.Users.Commands.DeactivateUser;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Users.Commands;

// PRUEBA DEL MÓDULO 1 (TENANTS Y SEGURIDAD - APLICACIÓN)
public class DeactivateUserHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly DeactivateUserHandler _handler;

    public DeactivateUserHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new DeactivateUserHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ConUsuarioExistente_DebeDesactivarloYGuardar()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var existingUser = new User("John", "Doe", "john@test.com", "hash", "Doctor", tenantId);
        _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var command = new DeactivateUserCommand(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingUser.IsActive.Should().BeFalse();
        _repositoryMock.Verify(r => r.UpdateAsync(existingUser, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConUsuarioInexistente_DebeLanzarException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var command = new DeactivateUserCommand(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Usuario no encontrado.");
    }
}
