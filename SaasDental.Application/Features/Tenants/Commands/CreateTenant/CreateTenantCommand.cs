using MediatR;

namespace SaasDental.Application.Features.Tenants.Commands.CreateTenant;

/// <summary>
/// Command to register a new Tenant (Clinic/Sede) in the system.
/// </summary>
public record CreateTenantCommand(string Name, string Address) : IRequest<CreateTenantResult>;

/// <summary>
/// Result returned after successfully creating a Tenant.
/// </summary>
public record CreateTenantResult(Guid Id, string Name, string Address, bool IsActive);
