using SaasDental.Domain.Entities;

namespace SaasDental.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT for the given user and their tenant.
    /// Claims include: sub, email, role, tenant_id, tenant_name, given_name, family_name.
    /// </summary>
    string GenerateToken(User user, string tenantName);
}
