using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Appointments.Commands.UpdateAppointmentStatus;

public record UpdateAppointmentStatusCommand(Guid Id, AppointmentStatus NewStatus) : IRequest;

public class UpdateAppointmentStatusValidator : AbstractValidator<UpdateAppointmentStatusCommand>
{
    public UpdateAppointmentStatusValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}

public class UpdateAppointmentStatusHandler : IRequestHandler<UpdateAppointmentStatusCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public UpdateAppointmentStatusHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (appointment == null)
            throw new Exception("Cita no encontrada.");

        appointment.ChangeStatus(request.NewStatus);

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
    }
}
