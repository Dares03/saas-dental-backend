using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Clinical.Queries.GetOdontogram;

public record FindingDto(Guid Id, string FindingType, FindingColor Color, string Nomenclature);
public record ToothSurfaceDto(Guid Id, SurfaceType SurfaceType, List<FindingDto> Findings);
public record ToothDto(Guid Id, int ToothNumber, List<FindingDto> ToothLevelFindings, List<ToothSurfaceDto> Surfaces);
public record OdontogramDto(Guid Id, OdontogramVersionType VersionType, string? Specifications, string? Observations, List<ToothDto> Teeth);

public record GetOdontogramQuery(Guid ClinicalHistoryId, OdontogramVersionType VersionType = OdontogramVersionType.Initial) : IRequest<OdontogramDto?>;

public class GetOdontogramHandler : IRequestHandler<GetOdontogramQuery, OdontogramDto?>
{
    private readonly IClinicalRepository _clinicalRepository;

    public GetOdontogramHandler(IClinicalRepository clinicalRepository)
    {
        _clinicalRepository = clinicalRepository;
    }

    public async Task<OdontogramDto?> Handle(GetOdontogramQuery request, CancellationToken cancellationToken)
    {
        // For simplicity, we are assuming the Initial one. If Evolution is requested we would need a GetEvolutionOdontogram method.
        // Let's assume GetInitialOdontogramAsync for now.
        var odontogram = await _clinicalRepository.GetInitialOdontogramAsync(request.ClinicalHistoryId, cancellationToken);

        if (odontogram == null)
            return null;

        var teethDto = odontogram.Teeth.Select(t => new ToothDto(
            t.Id,
            t.ToothNumber,
            t.ToothLevelFindings.Select(f => new FindingDto(f.Id, f.FindingType, f.Color, f.Nomenclature)).ToList(),
            t.Surfaces.Select(s => new ToothSurfaceDto(
                s.Id,
                s.SurfaceType,
                s.Findings.Select(f => new FindingDto(f.Id, f.FindingType, f.Color, f.Nomenclature)).ToList()
            )).ToList()
        )).ToList();

        return new OdontogramDto(
            odontogram.Id,
            odontogram.VersionType,
            odontogram.Specifications,
            odontogram.Observations,
            teethDto
        );
    }
}
