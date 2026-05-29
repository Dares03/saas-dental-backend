using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class PatientDebt : BaseEntity
{
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal RemainingAmount => TotalAmount - PaidAmount;
    public DebtStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    // Optional linking to a specific appointment or service if needed
    public Guid? AppointmentId { get; private set; }
    public Appointment? Appointment { get; private set; }

    public Guid TenantId { get; private set; }

    private PatientDebt() { }

    public PatientDebt(decimal totalAmount, string description, Guid patientId, Guid? appointmentId, Guid tenantId)
    {
        TotalAmount = totalAmount;
        PaidAmount = 0;
        Status = DebtStatus.Pending;
        Description = description;
        PatientId = patientId;
        AppointmentId = appointmentId;
        TenantId = tenantId;
    }

    public void AddPayment(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");
        
        PaidAmount += amount;
        
        if (PaidAmount >= TotalAmount)
        {
            Status = DebtStatus.Paid;
        }
        else if (PaidAmount > 0)
        {
            Status = DebtStatus.Partial;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePayment(decimal amount)
    {
        PaidAmount -= amount;
        
        if (PaidAmount <= 0)
        {
            PaidAmount = 0;
            Status = DebtStatus.Pending;
        }
        else
        {
            Status = DebtStatus.Partial;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
