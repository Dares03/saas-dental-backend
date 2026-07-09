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
    string? FamilyHistory,
    string? PersonalHistory,
    Guid? InitialOdontogramId,
    List<ClinicalEvolutionDto> Evolutions);

public record ClinicalEvolutionDto(
    Guid Id,
    DateTime Date,
    string Description,
    Guid? ToothId,
    int? ToothNumber,
    string DoctorName,
    string? CurrentIllnessReason,
    string? CurrentIllnessStory,
    string? BloodPressure,
    string? HeartRate,
    string? Temperature,
    string? RespiratoryRate,
    string? GeneralClinicalExam);

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
            history.FamilyHistory,
            history.PersonalHistory,
            initialOdontogram?.Id,
            history.Evolutions.Select(e => new ClinicalEvolutionDto(
                e.Id,
                e.Date,
                e.Description,
                e.ToothId,
                e.Tooth?.ToothNumber, // Tooth was not included in ClinicalRepository! Wait! I must include it!
                e.CreatedByUser.FirstName + " " + e.CreatedByUser.LastName,
                e.CurrentIllnessReason,
                e.CurrentIllnessStory,
                e.BloodPressure,
                e.HeartRate,
                e.Temperature,
                e.RespiratoryRate,
                e.GeneralClinicalExam
            )).ToList()
        );
    }
}
