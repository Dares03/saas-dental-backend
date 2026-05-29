using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Patients.Queries.GetPatients;

public record PatientDto(Guid Id, string FirstName, string LastName, string? DocumentId, string? PhoneNumber, string? Email, bool IsActive);

public record GetPatientsQuery : IRequest<List<PatientDto>>;

public class GetPatientsHandler : IRequestHandler<GetPatientsQuery, List<PatientDto>>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientsHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<List<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.GetAllAsync(cancellationToken);

        return patients.Select(p => new PatientDto(
            p.Id,
            p.FirstName,
            p.LastName,
            p.DocumentId,
            p.PhoneNumber,
            p.Email,
            p.IsActive
        )).ToList();
    }
}
