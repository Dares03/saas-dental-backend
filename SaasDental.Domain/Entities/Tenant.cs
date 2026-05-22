using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Navigation property for Users
    public ICollection<User> Users { get; private set; } = new List<User>();

    private Tenant() { } // For EF Core

    public Tenant(string name, string address)
    {
        Name = name;
        Address = address;
        IsActive = true;
    }

    public void UpdateDetails(string name, string address)
    {
        Name = name;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
