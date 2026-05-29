using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Appointments.Queries.GetAppointmentsByBranch;

public record AppointmentDto(
    Guid Id, 
    DateTime Date, 
    int DurationMinutes, 
    string Reason, 
    AppointmentStatus Status, 
    string PatientFullName, 
    string DentistFullName);

public record GetAppointmentsByBranchQuery(Guid BranchId, DateTime StartDate, DateTime EndDate) : IRequest<List<AppointmentDto>>;

public class GetAppointmentsByBranchHandler : IRequestHandler<GetAppointmentsByBranchQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByBranchHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByBranchQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetByBranchAsync(request.BranchId, request.StartDate, request.EndDate, cancellationToken);

        return appointments.Select(a => new AppointmentDto(
            a.Id,
            a.Date,
            a.DurationMinutes,
            a.Reason,
            a.Status,
            $"{a.Patient.FirstName} {a.Patient.LastName}",
            $"{a.Dentist.FirstName} {a.Dentist.LastName}"
        )).ToList();
    }
}
