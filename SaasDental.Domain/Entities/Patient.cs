using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class Patient : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? DocumentId { get; private set; } // DNI/Passport (Optional initially)
    public DateTime? DateOfBirth { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }

    // Multitenancy: The Tenant this patient belongs to
    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;

    // Navigation properties
    public ICollection<PatientRelative> Relatives { get; private set; } = new List<PatientRelative>();
    // public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    private Patient() { } // For EF Core

    public Patient(string firstName, string lastName, string? documentId, DateTime? dateOfBirth, string? phoneNumber, string? email, Guid tenantId)
    {
        FirstName = firstName;
        LastName = lastName;
        DocumentId = documentId;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Email = email;
        TenantId = tenantId;
        IsActive = true;
    }

    public void UpdateDetails(string firstName, string lastName, string? documentId, DateTime? dateOfBirth, string? phoneNumber, string? email)
    {
        FirstName = firstName;
        LastName = lastName;
        DocumentId = documentId;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
