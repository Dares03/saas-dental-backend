using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Financial.Queries.GetCashRegisterTransactions;

public record CashTransactionDto(
    Guid Id,
    int Type,
    decimal Amount,
    string Reason,
    int PaymentMethod,
    bool IsVoided,
    DateTime CreatedAt);

public record GetCashRegisterTransactionsQuery(Guid CashRegisterId) : IRequest<List<CashTransactionDto>>;

public class GetCashRegisterTransactionsHandler : IRequestHandler<GetCashRegisterTransactionsQuery, List<CashTransactionDto>>
{
    private readonly IFinancialRepository _financialRepository;

    public GetCashRegisterTransactionsHandler(IFinancialRepository financialRepository)
    {
        _financialRepository = financialRepository;
    }

    public async Task<List<CashTransactionDto>> Handle(GetCashRegisterTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _financialRepository.GetTransactionsByCashRegisterAsync(request.CashRegisterId, cancellationToken);

        return transactions.Select(t => new CashTransactionDto(
            t.Id,
            (int)t.Type,
            t.Amount,
            t.Reason,
            (int)t.PaymentMethod,
            t.IsVoided,
            t.CreatedAt
        )).ToList();
    }
}
