using Microsoft.AspNetCore.Http;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Infrastructure.Services;

/// <summary>
/// Reads the current TenantId from the "tenant_id" claim in the JWT token.
/// Replaces the MockTenantService once authentication is active.
/// </summary>
public class HttpContextTenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetCurrentTenantId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");
        if (claim is null) return null;
        return Guid.TryParse(claim.Value, out var tenantId) ? tenantId : null;
    }

    public Guid? GetCurrentUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim is null) return null;
        return Guid.TryParse(claim.Value, out var userId) ? userId : null;
    }
}
