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

    // Enfermedad Actual (Can be stored as JSON or simple text if basic)
    public string? CurrentIllnessReason { get; private set; }
    public string? CurrentIllnessStory { get; private set; }

    // Antecedentes (JSON or simple text)
    public string? FamilyHistory { get; private set; }
    public string? PersonalHistory { get; private set; }

    // Exploración Física Básica
    public string? BloodPressure { get; private set; }
    public string? HeartRate { get; private set; }
    public string? Temperature { get; private set; }
    public string? RespiratoryRate { get; private set; }
    public string? GeneralClinicalExam { get; private set; }

    // Relación
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    // Odontogramas asociados
    public ICollection<Odontogram> Odontograms { get; private set; } = new List<Odontogram>();

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

    public void UpdateIllnessAndHistory(string? reason, string? story, string? familyHistory, string? personalHistory)
    {
        CurrentIllnessReason = reason;
        CurrentIllnessStory = story;
        FamilyHistory = familyHistory;
        PersonalHistory = personalHistory;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePhysicalExam(string? bloodPressure, string? heartRate, string? temperature, string? respiratoryRate, string? generalClinicalExam)
    {
        BloodPressure = bloodPressure;
        HeartRate = heartRate;
        Temperature = temperature;
        RespiratoryRate = respiratoryRate;
        GeneralClinicalExam = generalClinicalExam;
        UpdatedAt = DateTime.UtcNow;
    }
}
