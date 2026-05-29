using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Multitenancy: The Tenant this branch belongs to
    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;

    private Branch() { } // For EF Core

    public Branch(string name, string address, string phoneNumber, Guid tenantId)
    {
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        TenantId = tenantId;
        IsActive = true;
    }

    public void UpdateDetails(string name, string address, string phoneNumber)
    {
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
