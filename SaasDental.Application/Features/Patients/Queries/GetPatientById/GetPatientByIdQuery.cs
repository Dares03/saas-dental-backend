using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Patients.Queries.GetPatientById;

public record PatientDetailDto(
    Guid Id, string FirstName, string LastName, string? DocumentId, 
    DateTime? DateOfBirth, string? PhoneNumber, string? Email, 
    string? Gender, string? Address, bool IsActive, DateTime CreatedAt,
    List<PatientRelativeDto> Relatives);

public record PatientRelativeDto(Guid Id, string FullName, string Relationship, string? PhoneNumber, bool IsEmergencyContact);

public record GetPatientByIdQuery(Guid PatientId) : IRequest<PatientDetailDto?>;

public class GetPatientByIdHandler : IRequestHandler<GetPatientByIdQuery, PatientDetailDto?>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientByIdHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDetailDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient == null) return null;

        return new PatientDetailDto(
            patient.Id, patient.FirstName, patient.LastName, patient.DocumentId,
            patient.DateOfBirth, patient.PhoneNumber, patient.Email,
            patient.Gender, patient.Address, patient.IsActive, patient.CreatedAt,
            patient.Relatives.Select(r => new PatientRelativeDto(
                r.Id, r.FullName, r.Relationship, r.PhoneNumber, r.IsEmergencyContact
            )).ToList());
    }
}
