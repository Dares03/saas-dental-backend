using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class ClinicalHistory : BaseEntity
{
    // Datos de Filiación que no están en Patient
    public string? Occupation { get; private set; }
    public string? Religion { get; private set; }
    public string? MaritalStatus { get; private set; }
    public string? PlaceOfOrigin { get; private set; }
    public string? CompanionName { get; private set; }

    // Antecedentes (JSON or simple text)
    public string? FamilyHistory { get; private set; }
    public string? PersonalHistory { get; private set; }

    // Relación
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    // Odontogramas asociados
    public ICollection<Odontogram> Odontograms { get; private set; } = new List<Odontogram>();

    // Evoluciones Clínicas asociadas
    public ICollection<ClinicalEvolution> Evolutions { get; private set; } = new List<ClinicalEvolution>();

    private ClinicalHistory() { }

    public ClinicalHistory(Guid patientId)
    {
        PatientId = patientId;
    }

    public void UpdateIdentification(string? occupation, string? religion, string? maritalStatus, string? placeOfOrigin, string? companionName)
    {
        Occupation = occupation;
        Religion = religion;
        MaritalStatus = maritalStatus;
        PlaceOfOrigin = placeOfOrigin;
        CompanionName = companionName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateHistory(string? familyHistory, string? personalHistory)
    {
        FamilyHistory = familyHistory;
        PersonalHistory = personalHistory;
        UpdatedAt = DateTime.UtcNow;
    }


}
