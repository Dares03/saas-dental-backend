using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class PatientRelative : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty; // e.g. Father, Mother, Sibling
    public string? PhoneNumber { get; private set; }
    public bool IsEmergencyContact { get; private set; }

    // Relationship to Patient
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    private PatientRelative() { } // For EF Core

    public PatientRelative(string fullName, string relationship, string? phoneNumber, bool isEmergencyContact, Guid patientId)
    {
        FullName = fullName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
        IsEmergencyContact = isEmergencyContact;
        PatientId = patientId;
    }

    public void UpdateDetails(string fullName, string relationship, string? phoneNumber, bool isEmergencyContact)
    {
        FullName = fullName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
        IsEmergencyContact = isEmergencyContact;
        UpdatedAt = DateTime.UtcNow;
    }
}
