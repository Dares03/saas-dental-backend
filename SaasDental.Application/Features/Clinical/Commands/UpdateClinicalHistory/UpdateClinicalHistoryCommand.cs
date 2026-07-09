using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Clinical.Commands.UpdateClinicalHistory;

public record UpdateClinicalHistoryCommand(
    Guid PatientId,
    string? Occupation,
    string? Religion,
    string? MaritalStatus,
    string? PlaceOfOrigin,
    string? CompanionName,
    string? FamilyHistory,
    string? PersonalHistory) : IRequest<Guid>;

public class UpdateClinicalHistoryValidator : AbstractValidator<UpdateClinicalHistoryCommand>
{
    public UpdateClinicalHistoryValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}

public class UpdateClinicalHistoryHandler : IRequestHandler<UpdateClinicalHistoryCommand, Guid>
{
    private readonly IClinicalRepository _clinicalRepository;

    public UpdateClinicalHistoryHandler(IClinicalRepository clinicalRepository)
    {
        _clinicalRepository = clinicalRepository;
    }

    public async Task<Guid> Handle(UpdateClinicalHistoryCommand request, CancellationToken cancellationToken)
    {
        var history = await _clinicalRepository.GetHistoryByPatientIdAsync(request.PatientId, cancellationToken);

        if (history == null)
            throw new Exception("Historia clínica no encontrada para este paciente.");

        history.UpdateIdentification(request.Occupation, request.Religion, request.MaritalStatus, request.PlaceOfOrigin, request.CompanionName);
        history.UpdateHistory(request.FamilyHistory, request.PersonalHistory);

        await _clinicalRepository.UpdateHistoryAsync(history, cancellationToken);

        return history.Id;
    }
}
