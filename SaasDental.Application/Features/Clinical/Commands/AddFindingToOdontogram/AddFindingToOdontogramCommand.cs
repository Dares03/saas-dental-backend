using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Clinical.Commands.AddFindingToOdontogram;

public record AddFindingToOdontogramCommand(
    Guid ToothId,
    Guid? ToothSurfaceId,
    string FindingType, // e.g. Caries, Corona
    FindingColor Color,
    string Nomenclature) : IRequest<Guid>;

public class AddFindingToOdontogramValidator : AbstractValidator<AddFindingToOdontogramCommand>
{
    public AddFindingToOdontogramValidator()
    {
        RuleFor(x => x.ToothId).NotEmpty();
        RuleFor(x => x.FindingType).NotEmpty();
        RuleFor(x => x.Color).IsInEnum();
        RuleFor(x => x.Nomenclature).NotEmpty().MaximumLength(5);
    }
}

public class AddFindingToOdontogramHandler : IRequestHandler<AddFindingToOdontogramCommand, Guid>
{
    private readonly IClinicalRepository _clinicalRepository;

    public AddFindingToOdontogramHandler(IClinicalRepository clinicalRepository)
    {
        _clinicalRepository = clinicalRepository;
    }

    public async Task<Guid> Handle(AddFindingToOdontogramCommand request, CancellationToken cancellationToken)
    {
        ClinicalFinding finding;

        if (request.ToothSurfaceId.HasValue)
        {
            finding = new ClinicalFinding(
                request.FindingType, 
                request.Color, 
                request.Nomenclature, 
                request.ToothId, 
                request.ToothSurfaceId.Value);
        }
        else
        {
            finding = new ClinicalFinding(
                request.FindingType, 
                request.Color, 
                request.Nomenclature, 
                request.ToothId);
        }

        await _clinicalRepository.AddClinicalFindingAsync(finding, cancellationToken);

        return finding.Id;
    }
}
