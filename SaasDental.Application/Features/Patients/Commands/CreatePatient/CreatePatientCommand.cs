using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Patients.Commands.CreatePatient;

public record RelativeDto(string FullName, string Relationship, string? PhoneNumber, bool IsEmergencyContact);

public record CreatePatientCommand(
    string FirstName, 
    string LastName, 
    string? DocumentId, 
    DateTime? DateOfBirth, 
    string? PhoneNumber, 
    string? Email,
    List<RelativeDto>? Relatives) : IRequest<Guid>;

public class CreatePatientValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentId).MaximumLength(50);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IClinicalRepository _clinicalRepository;
    private readonly ITenantService _tenantService;

    public CreatePatientHandler(IPatientRepository patientRepository, IClinicalRepository clinicalRepository, ITenantService tenantService)
    {
        _patientRepository = patientRepository;
        _clinicalRepository = clinicalRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId()
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var patient = new Patient(
            request.FirstName, 
            request.LastName, 
            request.DocumentId, 
            request.DateOfBirth, 
            request.PhoneNumber, 
            request.Email, 
            tenantId);

        if (request.Relatives != null && request.Relatives.Any())
        {
            foreach (var rel in request.Relatives)
            {
                var relative = new PatientRelative(
                    rel.FullName, 
                    rel.Relationship, 
                    rel.PhoneNumber, 
                    rel.IsEmergencyContact, 
                    patient.Id);
                    
                patient.Relatives.Add(relative);
            }
        }

        await _patientRepository.AddAsync(patient, cancellationToken);
        
        // --- Added: Automatically create the Clinical History for the new patient ---
        var clinicalHistory = new ClinicalHistory(patient.Id);
        await _clinicalRepository.AddHistoryAsync(clinicalHistory, cancellationToken);

        return patient.Id;
    }
}
