using Microsoft.EntityFrameworkCore;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using SaasDental.Infrastructure.Persistence;

namespace SaasDental.Infrastructure.Repositories;

public class FinancialRepository : IFinancialRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FinancialRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddTreatmentServiceAsync(TreatmentService service, CancellationToken cancellationToken = default)
    {
        await _dbContext.TreatmentServices.AddAsync(service, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TreatmentService>> GetServicesByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TreatmentServices
            .Where(ts => ts.ServiceCategoryId == categoryId && ts.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<CashRegister?> GetActiveCashRegisterAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CashRegisters
            .FirstOrDefaultAsync(cr => cr.BranchId == branchId && cr.Status == CashRegisterStatus.Open, cancellationToken);
    }

    public async Task<CashRegister?> GetCashRegisterByIdAsync(Guid cashRegisterId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CashRegisters
            .Include(cr => cr.OpenedByUser)
            .FirstOrDefaultAsync(cr => cr.Id == cashRegisterId, cancellationToken);
    }

    public async Task<List<CashRegister>> GetCashRegistersHistoryAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CashRegisters
            .Include(cr => cr.OpenedByUser)
            .Where(cr => cr.BranchId == branchId)
            .OrderByDescending(cr => cr.OpenedAt)
            .Take(30) // limit to recent 30 to avoid huge payloads initially
            .ToListAsync(cancellationToken);
    }

    public async Task AddCashRegisterAsync(CashRegister cashRegister, CancellationToken cancellationToken = default)
    {
        await _dbContext.CashRegisters.AddAsync(cashRegister, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCashRegisterAsync(CashRegister cashRegister, CancellationToken cancellationToken = default)
    {
        _dbContext.CashRegisters.Update(cashRegister);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCashTransactionAsync(CashTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.CashTransactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CashTransaction?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CashTransactions
            .FirstOrDefaultAsync(ct => ct.Id == transactionId, cancellationToken);
    }

    public async Task<List<CashTransaction>> GetTransactionsByCashRegisterAsync(Guid cashRegisterId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CashTransactions
            .Where(ct => ct.CashRegisterId == cashRegisterId)
            .OrderByDescending(ct => ct.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateTransactionAsync(CashTransaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.CashTransactions.Update(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PatientDebt?> GetPatientDebtByIdAsync(Guid debtId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PatientDebts
            .FirstOrDefaultAsync(pd => pd.Id == debtId, cancellationToken);
    }

    public async Task AddPatientDebtAsync(PatientDebt debt, CancellationToken cancellationToken = default)
    {
        await _dbContext.PatientDebts.AddAsync(debt, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePatientDebtAsync(PatientDebt debt, CancellationToken cancellationToken = default)
    {
        _dbContext.PatientDebts.Update(debt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
