using SaasDental.Domain.Entities;

namespace SaasDental.Application.Common.Interfaces;

public interface IFinancialRepository
{
    // Catálogo
    Task AddTreatmentServiceAsync(TreatmentService service, CancellationToken cancellationToken = default);
    Task<List<TreatmentService>> GetServicesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    
    // Caja
    Task<CashRegister?> GetActiveCashRegisterAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<CashRegister?> GetCashRegisterByIdAsync(Guid cashRegisterId, CancellationToken cancellationToken = default);
    Task AddCashRegisterAsync(CashRegister cashRegister, CancellationToken cancellationToken = default);
    Task UpdateCashRegisterAsync(CashRegister cashRegister, CancellationToken cancellationToken = default);
    
    // Transacciones
    Task AddCashTransactionAsync(CashTransaction transaction, CancellationToken cancellationToken = default);
    Task<CashTransaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<List<CashTransaction>> GetTransactionsByCashRegisterAsync(Guid cashRegisterId, CancellationToken cancellationToken = default);
    Task UpdateTransactionAsync(CashTransaction transaction, CancellationToken cancellationToken = default);

    // Deudas
    Task<PatientDebt?> GetPatientDebtByIdAsync(Guid debtId, CancellationToken cancellationToken = default);
    Task AddPatientDebtAsync(PatientDebt debt, CancellationToken cancellationToken = default);
    Task UpdatePatientDebtAsync(PatientDebt debt, CancellationToken cancellationToken = default);
}
