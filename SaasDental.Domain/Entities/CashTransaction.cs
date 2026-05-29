using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class CashTransaction : BaseEntity
{
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; private set; }
    public bool IsVoided { get; private set; }

    public Guid CashRegisterId { get; private set; }
    public CashRegister CashRegister { get; private set; } = null!;

    public Guid? PatientDebtId { get; private set; }
    public PatientDebt? PatientDebt { get; private set; }

    public Guid TenantId { get; private set; }

    private CashTransaction() { }

    public CashTransaction(TransactionType type, decimal amount, string reason, PaymentMethod paymentMethod, Guid cashRegisterId, Guid? patientDebtId, Guid tenantId)
    {
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");

        Type = type;
        Amount = amount;
        Reason = reason;
        PaymentMethod = paymentMethod;
        IsVoided = false;
        CashRegisterId = cashRegisterId;
        PatientDebtId = patientDebtId;
        TenantId = tenantId;
    }

    public void VoidTransaction()
    {
        if (IsVoided) throw new InvalidOperationException("La transacción ya está anulada.");
        
        IsVoided = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
