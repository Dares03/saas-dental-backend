using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Financial.Commands.CreateTreatmentService;

public record CreateTreatmentServiceCommand(
    string Name,
    string? Description,
    decimal BasePrice,
    decimal DoctorCommissionPercentage,
    Guid ServiceCategoryId) : IRequest<Guid>;

public class CreateTreatmentServiceValidator : AbstractValidator<CreateTreatmentServiceCommand>
{
    public CreateTreatmentServiceValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DoctorCommissionPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.ServiceCategoryId).NotEmpty();
    }
}

public class CreateTreatmentServiceHandler : IRequestHandler<CreateTreatmentServiceCommand, Guid>
{
    private readonly IFinancialRepository _financialRepository;
    private readonly ITenantService _tenantService;

    public CreateTreatmentServiceHandler(IFinancialRepository financialRepository, ITenantService tenantService)
    {
        _financialRepository = financialRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(CreateTreatmentServiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var service = new TreatmentService(
            request.Name,
            request.Description,
            request.BasePrice,
            request.DoctorCommissionPercentage,
            request.ServiceCategoryId,
            tenantId);

        await _financialRepository.AddTreatmentServiceAsync(service, cancellationToken);

        return service.Id;
    }
}
