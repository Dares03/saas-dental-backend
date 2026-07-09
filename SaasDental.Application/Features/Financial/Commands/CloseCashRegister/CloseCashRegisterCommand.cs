using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Financial.Commands.CloseCashRegister;

public record CloseCashRegisterCommand(Guid CashRegisterId, decimal ReportedFinalBalance) : IRequest;

public class CloseCashRegisterValidator : AbstractValidator<CloseCashRegisterCommand>
{
    public CloseCashRegisterValidator()
    {
        RuleFor(v => v.CashRegisterId).NotEmpty();
        RuleFor(v => v.ReportedFinalBalance).GreaterThanOrEqualTo(0);
    }
}

public class CloseCashRegisterHandler : IRequestHandler<CloseCashRegisterCommand>
{
    private readonly IFinancialRepository _financialRepository;

    public CloseCashRegisterHandler(IFinancialRepository financialRepository)
    {
        _financialRepository = financialRepository;
    }

    public async Task Handle(CloseCashRegisterCommand request, CancellationToken cancellationToken)
    {
        var cashRegister = await _financialRepository.GetCashRegisterByIdAsync(request.CashRegisterId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la caja con ID {request.CashRegisterId}");

        cashRegister.Close(request.ReportedFinalBalance);

        await _financialRepository.UpdateCashRegisterAsync(cashRegister, cancellationToken);
    }
}
