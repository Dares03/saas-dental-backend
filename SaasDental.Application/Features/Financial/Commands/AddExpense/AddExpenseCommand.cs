using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Financial.Commands.AddExpense;

public record AddExpenseCommand(
    Guid CashRegisterId,
    decimal Amount,
    string Reason,
    int PaymentMethod) : IRequest<Guid>;

public class AddExpenseValidator : AbstractValidator<AddExpenseCommand>
{
    public AddExpenseValidator()
    {
        RuleFor(v => v.CashRegisterId).NotEmpty();
        RuleFor(v => v.Amount).GreaterThan(0);
        RuleFor(v => v.Reason).NotEmpty().MaximumLength(200);
        RuleFor(v => v.PaymentMethod).IsInEnum();
    }
}

public class AddExpenseHandler : IRequestHandler<AddExpenseCommand, Guid>
{
    private readonly IFinancialRepository _financialRepository;
    private readonly ITenantService _tenantService;

    public AddExpenseHandler(IFinancialRepository financialRepository, ITenantService tenantService)
    {
        _financialRepository = financialRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("Contexto sin Tenant válido.");

        var cashRegister = await _financialRepository.GetCashRegisterByIdAsync(request.CashRegisterId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la caja con ID {request.CashRegisterId}");

        if (cashRegister.Status != CashRegisterStatus.Open)
            throw new InvalidOperationException("No se pueden registrar egresos en una caja cerrada.");

        var transaction = new CashTransaction(
            TransactionType.Expense,
            request.Amount,
            request.Reason,
            (PaymentMethod)request.PaymentMethod,
            request.CashRegisterId,
            null,
            tenantId);

        cashRegister.AddTransaction(request.Amount, TransactionType.Expense);

        await _financialRepository.AddCashTransactionAsync(transaction, cancellationToken);
        await _financialRepository.UpdateCashRegisterAsync(cashRegister, cancellationToken);

        return transaction.Id;
    }
}
