using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Clinical.Queries.GetHistoryByPatientId;

public record ClinicalHistoryDto(
    Guid Id,
    Guid PatientId,
    string? Occupation,
    string? Religion,
    string? MaritalStatus,
    string? PlaceOfOrigin,
    string? CompanionName,
    string? CurrentIllnessReason,
    string? CurrentIllnessStory,
    string? FamilyHistory,
    string? PersonalHistory,
    string? BloodPressure,
    string? HeartRate,
    string? Temperature,
    string? RespiratoryRate,
    string? GeneralClinicalExam,
    Guid? InitialOdontogramId);

public record GetHistoryByPatientIdQuery(Guid PatientId) : IRequest<ClinicalHistoryDto?>;

public class GetHistoryByPatientIdHandler : IRequestHandler<GetHistoryByPatientIdQuery, ClinicalHistoryDto?>
{
    private readonly IClinicalRepository _clinicalRepository;

    public GetHistoryByPatientIdHandler(IClinicalRepository clinicalRepository)
    {
        _clinicalRepository = clinicalRepository;
    }

    public async Task<ClinicalHistoryDto?> Handle(GetHistoryByPatientIdQuery request, CancellationToken cancellationToken)
    {
        var history = await _clinicalRepository.GetHistoryByPatientIdAsync(request.PatientId, cancellationToken);

        if (history == null)
            return null;

        var initialOdontogram = history.Odontograms.FirstOrDefault(o => o.VersionType == Domain.Enums.OdontogramVersionType.Initial);

        return new ClinicalHistoryDto(
            history.Id,
            history.PatientId,
            history.Occupation,
            history.Religion,
            history.MaritalStatus,
            history.PlaceOfOrigin,
            history.CompanionName,
            history.CurrentIllnessReason,
            history.CurrentIllnessStory,
            history.FamilyHistory,
            history.PersonalHistory,
            history.BloodPressure,
            history.HeartRate,
            history.Temperature,
            history.RespiratoryRate,
            history.GeneralClinicalExam,
            initialOdontogram?.Id
        );
    }
}
