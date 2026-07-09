using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class ClinicalEvolution : BaseEntity
{
    public DateTime Date { get; private set; }
    public string Description { get; private set; }

    // Enfermedad Actual (Consulta)
    public string? CurrentIllnessReason { get; private set; }
    public string? CurrentIllnessStory { get; private set; }

    // Exploración Física Básica (Signos vitales de esta consulta)
    public string? BloodPressure { get; private set; }
    public string? HeartRate { get; private set; }
    public string? Temperature { get; private set; }
    public string? RespiratoryRate { get; private set; }
    public string? GeneralClinicalExam { get; private set; }
    
    // Si la evolución se asocia a la historia clínica en general
    public Guid ClinicalHistoryId { get; private set; }
    public ClinicalHistory ClinicalHistory { get; private set; } = null!;

    // Si la evolución se refiere específicamente a un diente del odontograma (opcional)
    public Guid? ToothId { get; private set; }
    public Tooth? Tooth { get; private set; }

    // El doctor/usuario que escribió la evolución
    public Guid CreatedByUserId { get; private set; }
    public User CreatedByUser { get; private set; } = null!;

    public Guid TenantId { get; private set; }

    private ClinicalEvolution() { }

    public ClinicalEvolution(
        DateTime date, 
        string description, 
        Guid clinicalHistoryId, 
        Guid? toothId, 
        Guid createdByUserId, 
        Guid tenantId,
        string? currentIllnessReason = null,
        string? currentIllnessStory = null,
        string? bloodPressure = null,
        string? heartRate = null,
        string? temperature = null,
        string? respiratoryRate = null,
        string? generalClinicalExam = null)
    {
        Date = date;
        Description = description;
        ClinicalHistoryId = clinicalHistoryId;
        ToothId = toothId;
        CreatedByUserId = createdByUserId;
        TenantId = tenantId;
        CurrentIllnessReason = currentIllnessReason;
        CurrentIllnessStory = currentIllnessStory;
        BloodPressure = bloodPressure;
        HeartRate = heartRate;
        Temperature = temperature;
        RespiratoryRate = respiratoryRate;
        GeneralClinicalExam = generalClinicalExam;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
