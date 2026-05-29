using MediatR;

namespace SaasDental.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user with email and password.
/// Returns a JWT token on success.
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(
    string Token,
    DateTime ExpiresAt,
    AuthUserDto User
);

public record AuthUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    Guid TenantId,
    string TenantName
);
