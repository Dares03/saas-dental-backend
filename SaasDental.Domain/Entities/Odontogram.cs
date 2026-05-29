using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class Odontogram : BaseEntity
{
    public OdontogramVersionType VersionType { get; private set; }
    public string? Specifications { get; private set; } // Field for writing textual specifications as per MINSA norm
    public string? Observations { get; private set; }

    public Guid ClinicalHistoryId { get; private set; }
    public ClinicalHistory ClinicalHistory { get; private set; } = null!;

    public ICollection<Tooth> Teeth { get; private set; } = new List<Tooth>();

    private Odontogram() { }

    public Odontogram(Guid clinicalHistoryId, OdontogramVersionType versionType)
    {
        ClinicalHistoryId = clinicalHistoryId;
        VersionType = versionType;
    }

    public void UpdateTextFields(string? specifications, string? observations)
    {
        Specifications = specifications;
        Observations = observations;
        UpdatedAt = DateTime.UtcNow;
    }
}
