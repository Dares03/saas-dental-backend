namespace SaasDental.Application.Common.Interfaces;

public interface ITenantService
{
    // Retrieves the current Tenant ID from the HTTP Context or User Claims
    Guid? GetCurrentTenantId();
    
    // Retrieves the current User ID from the HTTP Context or User Claims
    Guid? GetCurrentUserId();
}
