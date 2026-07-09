using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Financial.Commands.OpenCashRegister;

public record OpenCashRegisterCommand(
    Guid BranchId,
    decimal InitialBalance) : IRequest<Guid>;

public class OpenCashRegisterValidator : AbstractValidator<OpenCashRegisterCommand>
{
    public OpenCashRegisterValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.InitialBalance).GreaterThanOrEqualTo(0);
    }
}

public class OpenCashRegisterHandler : IRequestHandler<OpenCashRegisterCommand, Guid>
{
    private readonly IFinancialRepository _financialRepository;
    private readonly ITenantService _tenantService;

    public OpenCashRegisterHandler(IFinancialRepository financialRepository, ITenantService tenantService)
    {
        _financialRepository = financialRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(OpenCashRegisterCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        // Check if there is already an open cash register for this branch
        var activeRegister = await _financialRepository.GetActiveCashRegisterAsync(request.BranchId, cancellationToken);
        if (activeRegister != null)
        {
            throw new InvalidOperationException("Ya existe una caja abierta para esta sede.");
        }

        var currentUserId = _tenantService.GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

        var cashRegister = new CashRegister(
            request.InitialBalance,
            request.BranchId,
            currentUserId,
            tenantId);

        await _financialRepository.AddCashRegisterAsync(cashRegister, cancellationToken);

        return cashRegister.Id;
    }
}
