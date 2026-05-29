using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Financial.Commands.AddIncome;

public record AddIncomeCommand(
    Guid CashRegisterId,
    decimal Amount,
    string Reason,
    PaymentMethod PaymentMethod,
    Guid? PatientDebtId) : IRequest<Guid>;

public class AddIncomeValidator : AbstractValidator<AddIncomeCommand>
{
    public AddIncomeValidator()
    {
        RuleFor(x => x.CashRegisterId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PaymentMethod).IsInEnum();
    }
}

public class AddIncomeHandler : IRequestHandler<AddIncomeCommand, Guid>
{
    private readonly IFinancialRepository _financialRepository;
    private readonly ITenantService _tenantService;

    public AddIncomeHandler(IFinancialRepository financialRepository, ITenantService tenantService)
    {
        _financialRepository = financialRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(AddIncomeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var cashRegister = await _financialRepository.GetCashRegisterByIdAsync(request.CashRegisterId, cancellationToken);
        if (cashRegister == null)
            throw new Exception("Caja no encontrada.");

        if (cashRegister.Status != CashRegisterStatus.Open)
            throw new InvalidOperationException("La caja seleccionada está cerrada.");

        // Crear la transacción
        var transaction = new CashTransaction(
            TransactionType.Income,
            request.Amount,
            request.Reason,
            request.PaymentMethod,
            cashRegister.Id,
            request.PatientDebtId,
            tenantId);

        // Actualizar balance de la caja
        cashRegister.AddTransaction(request.Amount, TransactionType.Income);

        // Si el ingreso está pagando una deuda de paciente
        if (request.PatientDebtId.HasValue)
        {
            var debt = await _financialRepository.GetPatientDebtByIdAsync(request.PatientDebtId.Value, cancellationToken);
            if (debt != null)
            {
                debt.AddPayment(request.Amount);
                await _financialRepository.UpdatePatientDebtAsync(debt, cancellationToken);
            }
        }

        await _financialRepository.AddCashTransactionAsync(transaction, cancellationToken);
        await _financialRepository.UpdateCashRegisterAsync(cashRegister, cancellationToken);

        return transaction.Id;
    }
}
