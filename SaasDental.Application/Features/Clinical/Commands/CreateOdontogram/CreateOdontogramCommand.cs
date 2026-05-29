using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;

namespace SaasDental.Application.Features.Clinical.Commands.CreateOdontogram;

public record CreateOdontogramCommand(Guid ClinicalHistoryId, string? Observations) : IRequest<Guid>;

public class CreateOdontogramValidator : AbstractValidator<CreateOdontogramCommand>
{
    public CreateOdontogramValidator()
    {
        RuleFor(x => x.ClinicalHistoryId).NotEmpty();
    }
}

public class CreateOdontogramHandler : IRequestHandler<CreateOdontogramCommand, Guid>
{
    private readonly IClinicalRepository _clinicalRepository;

    public CreateOdontogramHandler(IClinicalRepository clinicalRepository)
    {
        _clinicalRepository = clinicalRepository;
    }

    public async Task<Guid> Handle(CreateOdontogramCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya existe un odontograma inicial
        var existingInitial = await _clinicalRepository.GetInitialOdontogramAsync(request.ClinicalHistoryId, cancellationToken);
        if (existingInitial != null)
        {
            throw new Exception("Esta historia clínica ya tiene un Odontograma Inicial. Debe crear evoluciones o modificar el actual.");
        }

        var odontogram = new Odontogram(request.ClinicalHistoryId, OdontogramVersionType.Initial);
        odontogram.UpdateTextFields(null, request.Observations);

        // Generar los 52 dientes (11-18, 21-28, 31-38, 41-48, 51-55, 61-65, 71-75, 81-85)
        var adultTeeth = new[] { 
            11, 12, 13, 14, 15, 16, 17, 18, 
            21, 22, 23, 24, 25, 26, 27, 28, 
            31, 32, 33, 34, 35, 36, 37, 38, 
            41, 42, 43, 44, 45, 46, 47, 48 
        };
        var childTeeth = new[] { 
            51, 52, 53, 54, 55, 
            61, 62, 63, 64, 65, 
            71, 72, 73, 74, 75, 
            81, 82, 83, 84, 85 
        };

        var allTeeth = adultTeeth.Concat(childTeeth);

        foreach (var toothNum in allTeeth)
        {
            var tooth = new Tooth(toothNum, odontogram.Id);
            
            // Generar las 5 caras principales
            tooth.Surfaces.Add(new ToothSurface(SurfaceType.Oclusal, tooth.Id));
            tooth.Surfaces.Add(new ToothSurface(SurfaceType.Vestibular, tooth.Id));
            tooth.Surfaces.Add(new ToothSurface(SurfaceType.Mesial, tooth.Id));
            tooth.Surfaces.Add(new ToothSurface(SurfaceType.Distal, tooth.Id));
            tooth.Surfaces.Add(new ToothSurface(SurfaceType.Lingual, tooth.Id));

            odontogram.Teeth.Add(tooth);
        }

        await _clinicalRepository.AddOdontogramAsync(odontogram, cancellationToken);

        return odontogram.Id;
    }
}
