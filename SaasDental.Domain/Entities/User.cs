using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    
    // Multitenancy: The Tenant this user belongs to
    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;

    private User() { } // For EF Core

    public User(string firstName, string lastName, string email, string passwordHash, string role, Guid tenantId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        TenantId = tenantId;
        IsActive = true;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }
}
