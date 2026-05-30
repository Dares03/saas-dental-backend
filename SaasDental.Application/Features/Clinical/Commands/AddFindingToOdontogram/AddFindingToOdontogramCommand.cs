using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaasDental.Application.Features.Clinical.Commands.AddFindingToOdontogram;

public record AddFindingToOdontogramCommand(
    Guid OdontogramId,
    int ToothNumber,
    SurfaceType? SurfaceType,
    string FindingType, // e.g. Caries, Corona
    FindingColor Color,
    string Nomenclature) : IRequest<Guid>;

public class AddFindingToOdontogramValidator : AbstractValidator<AddFindingToOdontogramCommand>
{
    public AddFindingToOdontogramValidator()
    {
        RuleFor(x => x.OdontogramId).NotEmpty();
        RuleFor(x => x.ToothNumber).GreaterThan(0);
        RuleFor(x => x.FindingType).NotEmpty();
        RuleFor(x => x.Color).IsInEnum();
        RuleFor(x => x.Nomenclature).NotEmpty().MaximumLength(10);
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
        var odontogram = await _clinicalRepository.GetOdontogramByIdAsync(request.OdontogramId, cancellationToken);
        if (odontogram == null)
            throw new InvalidOperationException($"No se encontró el odontograma con ID {request.OdontogramId}.");

        // 1. Find or create Tooth
        var tooth = odontogram.Teeth.FirstOrDefault(t => t.ToothNumber == request.ToothNumber);
        if (tooth == null)
        {
            tooth = new Tooth(request.ToothNumber, request.OdontogramId);
            odontogram.Teeth.Add(tooth);
        }

        // 2. Find or create Surface (if applicable)
        ToothSurface? surface = null;
        if (request.SurfaceType.HasValue)
        {
            surface = tooth.Surfaces.FirstOrDefault(s => s.SurfaceType == request.SurfaceType.Value);
            if (surface == null)
            {
                surface = new ToothSurface(request.SurfaceType.Value, tooth.Id);
                tooth.Surfaces.Add(surface);
            }
        }

        // 3. Create Finding
        ClinicalFinding finding;
        if (surface != null)
        {
            finding = new ClinicalFinding(
                request.FindingType, 
                request.Color, 
                request.Nomenclature, 
                tooth.Id, 
                surface.Id);
            surface.Findings.Add(finding);
        }
        else
        {
            finding = new ClinicalFinding(
                request.FindingType, 
                request.Color, 
                request.Nomenclature, 
                tooth.Id);
            tooth.ToothLevelFindings.Add(finding);
        }

        await _clinicalRepository.AddClinicalFindingAsync(finding, cancellationToken);
        await _clinicalRepository.SaveChangesAsync(cancellationToken);

        return finding.Id;
    }
}
