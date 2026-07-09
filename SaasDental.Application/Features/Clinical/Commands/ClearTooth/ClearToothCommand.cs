using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Clinical.Commands.ClearTooth;

public record ClearToothCommand(Guid OdontogramId, int ToothNumber) : IRequest<bool>;

public class ClearToothHandler : IRequestHandler<ClearToothCommand, bool>
{
    private readonly IClinicalRepository _clinicalRepository;

    public ClearToothHandler(IClinicalRepository clinicalRepository)
    {
        _clinicalRepository = clinicalRepository;
    }

    public async Task<bool> Handle(ClearToothCommand request, CancellationToken cancellationToken)
    {
        await _clinicalRepository.DeleteToothAsync(request.OdontogramId, request.ToothNumber, cancellationToken);
        return true;
    }
}
