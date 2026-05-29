using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class CashRegister : BaseEntity
{
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public decimal InitialBalance { get; private set; }
    public decimal CalculatedFinalBalance { get; private set; }
    public decimal? ReportedFinalBalance { get; private set; }
    public CashRegisterStatus Status { get; private set; }

    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = null!;

    public Guid OpenedByUserId { get; private set; }
    public User OpenedByUser { get; private set; } = null!;

    public Guid TenantId { get; private set; }

    public ICollection<CashTransaction> Transactions { get; private set; } = new List<CashTransaction>();

    private CashRegister() { }

    public CashRegister(decimal initialBalance, Guid branchId, Guid openedByUserId, Guid tenantId)
    {
        OpenedAt = DateTime.UtcNow;
        InitialBalance = initialBalance;
        CalculatedFinalBalance = initialBalance;
        Status = CashRegisterStatus.Open;
        BranchId = branchId;
        OpenedByUserId = openedByUserId;
        TenantId = tenantId;
    }

    public void AddTransaction(decimal amount, TransactionType type)
    {
        if (Status != CashRegisterStatus.Open)
            throw new InvalidOperationException("No se pueden registrar transacciones en una caja cerrada.");

        if (type == TransactionType.Income)
            CalculatedFinalBalance += amount;
        else if (type == TransactionType.Expense)
            CalculatedFinalBalance -= amount;
    }

    public void RevertTransaction(decimal amount, TransactionType type)
    {
        if (Status != CashRegisterStatus.Open)
            throw new InvalidOperationException("No se pueden revertir transacciones en una caja cerrada.");

        // Reverse the operation
        if (type == TransactionType.Income)
            CalculatedFinalBalance -= amount;
        else if (type == TransactionType.Expense)
            CalculatedFinalBalance += amount;
    }

    public void Close(decimal reportedFinalBalance)
    {
        if (Status == CashRegisterStatus.Closed)
            throw new InvalidOperationException("La caja ya se encuentra cerrada.");

        ClosedAt = DateTime.UtcNow;
        ReportedFinalBalance = reportedFinalBalance;
        Status = CashRegisterStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }
}
