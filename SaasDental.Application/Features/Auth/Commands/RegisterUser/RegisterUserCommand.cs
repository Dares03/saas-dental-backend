using MediatR;

namespace SaasDental.Application.Features.Auth.Commands.RegisterUser;

/// <summary>
/// Command to create a new user within a specific tenant.
/// In production this endpoint should be protected (Admin only).
/// </summary>
public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role,
    Guid TenantId
) : IRequest<RegisterUserResult>;

public record RegisterUserResult(Guid Id, string Email, string Role, Guid TenantId);
