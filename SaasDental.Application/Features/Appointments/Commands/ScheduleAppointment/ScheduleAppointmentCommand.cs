using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Appointments.Commands.ScheduleAppointment;

public record ScheduleAppointmentCommand(
    Guid PatientId,
    Guid DentistId,
    Guid BranchId,
    DateTime Date,
    int DurationMinutes,
    string Reason) : IRequest<Guid>;

public class ScheduleAppointmentValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DentistId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Date).GreaterThan(DateTime.UtcNow.AddMinutes(-5)).WithMessage("La cita no puede ser en el pasado.");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(480); // Max 8 hours
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(255);
    }
}

public class ScheduleAppointmentHandler : IRequestHandler<ScheduleAppointmentCommand, Guid>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITenantService _tenantService;

    public ScheduleAppointmentHandler(IAppointmentRepository appointmentRepository, ITenantService tenantService)
    {
        _appointmentRepository = appointmentRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        // TODO: En el futuro agregar lógica para evitar superposición de citas para el mismo doctor.

        var appointment = new Appointment(
            request.Date,
            request.DurationMinutes,
            request.Reason,
            request.PatientId,
            request.DentistId,
            request.BranchId,
            tenantId);

        await _appointmentRepository.AddAsync(appointment, cancellationToken);

        return appointment.Id;
    }
}
