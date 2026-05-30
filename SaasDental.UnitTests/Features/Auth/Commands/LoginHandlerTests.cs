using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Common.Settings;
using SaasDental.Application.Features.Auth.Commands.Login;
using SaasDental.Domain.Entities;
using Xunit;

namespace SaasDental.UnitTests.Features.Auth.Commands;

// PRUEBA DEL MÓDULO 1 (ADMINISTRACIÓN Y ACCESOS): Validaciones de autenticación de usuarios y seguridad.
public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly IOptions<JwtSettings> _jwtSettings;

    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        
        var jwtSettings = new JwtSettings { ExpiryMinutes = 60 };
        _jwtSettings = Options.Create(jwtSettings);

        _handler = new LoginHandler(
            _userRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _jwtGeneratorMock.Object,
            _passwordHasherMock.Object,
            _jwtSettings
        );
    }

    // Verifica que cuando un usuario proporciona el correo y contraseña correctos,
    // el sistema orqueste la generación de un Token JWT y lo retorne correctamente.
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnLoginResult()
    {
        // Arrange
        var email = "admin@clinic.com";
        var password = "Password123!";
        var tenantId = Guid.NewGuid();
        
        var user = new User("Admin", "User", email, "hashed_password", "Admin", tenantId);
        var tenant = new Tenant("Test Clinic", "Address");
        
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email.ToLower(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
            
        _passwordHasherMock.Setup(h => h.Verify(password, "hashed_password"))
            .Returns(true);
            
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
            
        _jwtGeneratorMock.Setup(j => j.GenerateToken(user, tenant.Name))
            .Returns("valid_jwt_token");

        var command = new LoginCommand(email, password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("valid_jwt_token");
        result.User.Email.Should().Be(email);
        result.User.TenantName.Should().Be("Test Clinic");
    }

    // Valida que si la contraseña es incorrecta (el Hash falla), la capa de aplicación
    // impida el acceso y lance un UnauthorizedAccessException protegiendo el sistema.
    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var email = "admin@clinic.com";
        var password = "WrongPassword!";
        var tenantId = Guid.NewGuid();
        
        var user = new User("Admin", "User", email, "hashed_password", "Admin", tenantId);
        
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email.ToLower(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
            
        _passwordHasherMock.Setup(h => h.Verify(password, "hashed_password"))
            .Returns(false); // Invalid password

        var command = new LoginCommand(email, password);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }
}
