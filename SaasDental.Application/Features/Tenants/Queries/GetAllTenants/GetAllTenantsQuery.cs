using MediatR;

namespace SaasDental.Application.Features.Tenants.Queries.GetAllTenants;

/// <summary>
/// Query to retrieve all registered tenants (clinics).
/// </summary>
public record GetAllTenantsQuery : IRequest<IEnumerable<TenantDto>>;

/// <summary>
/// DTO representing a Tenant in read operations.
/// </summary>
public record TenantDto(Guid Id, string Name, string Address, bool IsActive, DateTime CreatedAt);
