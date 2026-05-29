using MediatR;

namespace SaasDental.Application.Features.Tenants.Queries.GetTenantById;

/// <summary>
/// Query to retrieve a single Tenant by its unique identifier.
/// </summary>
public record GetTenantByIdQuery(Guid Id) : IRequest<TenantDetailDto?>;

/// <summary>
/// Detailed DTO for a single Tenant, including user count.
/// </summary>
public record TenantDetailDto(Guid Id, string Name, string Address, bool IsActive, DateTime CreatedAt, int UserCount);
