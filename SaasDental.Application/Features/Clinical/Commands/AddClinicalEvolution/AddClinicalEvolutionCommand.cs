using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Clinical.Commands.AddClinicalEvolution;

public record AddClinicalEvolutionCommand(
    Guid ClinicalHistoryId,
    string Description,
    Guid? ToothId,
    string? CurrentIllnessReason = null,
    string? CurrentIllnessStory = null,
    string? BloodPressure = null,
    string? HeartRate = null,
    string? Temperature = null,
    string? RespiratoryRate = null,
    string? GeneralClinicalExam = null) : IRequest<Guid>;

public class AddClinicalEvolutionHandler : IRequestHandler<AddClinicalEvolutionCommand, Guid>
{
    private readonly IClinicalRepository _clinicalRepository;
    private readonly ITenantService _tenantService;

    public AddClinicalEvolutionHandler(IClinicalRepository clinicalRepository, ITenantService tenantService)
    {
        _clinicalRepository = clinicalRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(AddClinicalEvolutionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId() 
            ?? throw new UnauthorizedAccessException("Tenant inválido.");
            
        var userId = _tenantService.GetCurrentUserId() 
            ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

        var evolution = new ClinicalEvolution(
            DateTime.UtcNow,
            request.Description,
            request.ClinicalHistoryId,
            request.ToothId,
            userId,
            tenantId,
            request.CurrentIllnessReason,
            request.CurrentIllnessStory,
            request.BloodPressure,
            request.HeartRate,
            request.Temperature,
            request.RespiratoryRate,
            request.GeneralClinicalExam
        );

        await _clinicalRepository.AddClinicalEvolutionAsync(evolution, cancellationToken);
        return evolution.Id;
    }
}
