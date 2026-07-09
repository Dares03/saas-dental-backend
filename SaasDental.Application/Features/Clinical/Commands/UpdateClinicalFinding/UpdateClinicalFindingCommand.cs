using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaasDental.Application.Features.Clinical.Commands.UpdateClinicalFinding;

public record UpdateClinicalFindingCommand(
    Guid FindingId,
    FindingColor Color,
    string? EvolutionNote) : IRequest<bool>;

public class UpdateClinicalFindingValidator : AbstractValidator<UpdateClinicalFindingCommand>
{
    public UpdateClinicalFindingValidator()
    {
        RuleFor(x => x.FindingId).NotEmpty();
        RuleFor(x => x.Color).IsInEnum();
    }
}

public class UpdateClinicalFindingHandler : IRequestHandler<UpdateClinicalFindingCommand, bool>
{
    private readonly IClinicalRepository _clinicalRepository;
    private readonly ITenantService _tenantService;
    public UpdateClinicalFindingHandler(IClinicalRepository clinicalRepository, ITenantService tenantService)
    {
        _clinicalRepository = clinicalRepository;
        _tenantService = tenantService;
    }

    public async Task<bool> Handle(UpdateClinicalFindingCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId() 
            ?? throw new UnauthorizedAccessException("Tenant inválido.");
            
        var userId = _tenantService.GetCurrentUserId() 
            ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

        var finding = await _clinicalRepository.GetClinicalFindingByIdAsync(request.FindingId, cancellationToken);
        if (finding == null)
            throw new InvalidOperationException($"Hallazgo clínico {request.FindingId} no encontrado.");

        finding.UpdateColor(request.Color);

        // Si el usuario proporcionó una nota de evolución al cambiar el estado del hallazgo
        if (!string.IsNullOrWhiteSpace(request.EvolutionNote))
        {
            // Necesitamos saber la historia clínica para crear la evolución
            // ToothId -> Tooth -> Odontogram -> ClinicalHistoryId
            var tooth = await _clinicalRepository.GetToothByIdAsync(finding.ToothId.Value, cancellationToken);
                
            if (tooth != null)
            {
                var evolution = new ClinicalEvolution(
                    DateTime.UtcNow,
                    request.EvolutionNote,
                    tooth.Odontogram.ClinicalHistoryId,
                    tooth.Id,
                    userId,
                    tenantId
                );
                await _clinicalRepository.AddClinicalEvolutionAsync(evolution, cancellationToken);
            }
        }

        await _clinicalRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
