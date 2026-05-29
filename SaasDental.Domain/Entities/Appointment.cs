using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class Appointment : BaseEntity
{
    public DateTime Date { get; private set; }
    public int DurationMinutes { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public AppointmentStatus Status { get; private set; }

    // Relationship to Patient
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    // Relationship to Dentist (User)
    public Guid DentistId { get; private set; }
    public User Dentist { get; private set; } = null!;

    // Relationship to Branch (Sede)
    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = null!;

    // Multitenancy
    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;

    private Appointment() { } // For EF Core

    public Appointment(DateTime date, int durationMinutes, string reason, Guid patientId, Guid dentistId, Guid branchId, Guid tenantId)
    {
        Date = date;
        DurationMinutes = durationMinutes;
        Reason = reason;
        Status = AppointmentStatus.Scheduled;
        PatientId = patientId;
        DentistId = dentistId;
        BranchId = branchId;
        TenantId = tenantId;
    }

    public void UpdateDetails(DateTime date, int durationMinutes, string reason, string? notes)
    {
        Date = date;
        DurationMinutes = durationMinutes;
        Reason = reason;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(AppointmentStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
