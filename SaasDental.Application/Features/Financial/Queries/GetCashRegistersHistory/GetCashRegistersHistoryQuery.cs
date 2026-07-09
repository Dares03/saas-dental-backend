using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Financial.Queries.GetCashRegistersHistory;

public record CashRegisterHistoryDto(
    Guid Id, 
    DateTime OpenedAt, 
    DateTime? ClosedAt, 
    decimal InitialBalance, 
    decimal CalculatedFinalBalance, 
    decimal? ReportedFinalBalance, 
    int Status, 
    string OpenedByUserName);

public record GetCashRegistersHistoryQuery(Guid BranchId) : IRequest<List<CashRegisterHistoryDto>>;

public class GetCashRegistersHistoryHandler : IRequestHandler<GetCashRegistersHistoryQuery, List<CashRegisterHistoryDto>>
{
    private readonly IFinancialRepository _financialRepository;

    public GetCashRegistersHistoryHandler(IFinancialRepository financialRepository)
    {
        _financialRepository = financialRepository;
    }

    public async Task<List<CashRegisterHistoryDto>> Handle(GetCashRegistersHistoryQuery request, CancellationToken cancellationToken)
    {
        var registers = await _financialRepository.GetCashRegistersHistoryAsync(request.BranchId, cancellationToken);
        
        return registers.Select(r => new CashRegisterHistoryDto(
            r.Id,
            r.OpenedAt,
            r.ClosedAt,
            r.InitialBalance,
            r.CalculatedFinalBalance,
            r.ReportedFinalBalance,
            (int)r.Status,
            r.OpenedByUser?.FirstName + " " + r.OpenedByUser?.LastName
        )).ToList();
    }
}
