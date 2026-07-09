using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Patients.Commands.UpdatePatient;

public record UpdatePatientCommand(
    Guid PatientId,
    string FirstName,
    string LastName,
    string? DocumentId,
    DateTime? DateOfBirth,
    string? PhoneNumber,
    string? Email,
    string? Gender,
    string? Address) : IRequest<bool>;

public class UpdatePatientValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentId).MaximumLength(50);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class UpdatePatientHandler : IRequestHandler<UpdatePatientCommand, bool>
{
    private readonly IPatientRepository _patientRepository;

    public UpdatePatientHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken)
            ?? throw new KeyNotFoundException($"Paciente con ID '{request.PatientId}' no encontrado.");

        patient.UpdateDetails(
            request.FirstName, request.LastName, request.DocumentId,
            request.DateOfBirth, request.PhoneNumber, request.Email,
            request.Gender, request.Address);

        await _patientRepository.UpdateAsync(patient, cancellationToken);
        return true;
    }
}
