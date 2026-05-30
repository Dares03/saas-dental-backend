using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaasDental.Application.Features.Financial.Queries.GetActiveCashRegister;

public record GetActiveCashRegisterQuery(Guid BranchId) : IRequest<CashRegisterDto?>;

public class CashRegisterDto
{
    public Guid Id { get; set; }
    public DateTime OpenedAt { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CalculatedFinalBalance { get; set; }
    public CashRegisterStatus Status { get; set; }
    public Guid BranchId { get; set; }
}

public class GetActiveCashRegisterHandler : IRequestHandler<GetActiveCashRegisterQuery, CashRegisterDto?>
{
    private readonly IFinancialRepository _financialRepository;

    public GetActiveCashRegisterHandler(IFinancialRepository financialRepository)
    {
        _financialRepository = financialRepository;
    }

    public async Task<CashRegisterDto?> Handle(GetActiveCashRegisterQuery request, CancellationToken cancellationToken)
    {
        var register = await _financialRepository.GetActiveCashRegisterAsync(request.BranchId, cancellationToken);

        if (register == null)
            return null;

        return new CashRegisterDto
        {
            Id = register.Id,
            OpenedAt = register.OpenedAt,
            InitialBalance = register.InitialBalance,
            CalculatedFinalBalance = register.CalculatedFinalBalance,
            Status = register.Status,
            BranchId = register.BranchId
        };
    }
}
