using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Branches.Queries.GetBranches;

public record BranchDto(Guid Id, string Name, string Address, string PhoneNumber, bool IsActive);

public record GetBranchesQuery : IRequest<List<BranchDto>>;

public class GetBranchesHandler : IRequestHandler<GetBranchesQuery, List<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;

    public GetBranchesHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<List<BranchDto>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);

        // Map to DTO
        return branches.Select(b => new BranchDto(
            b.Id,
            b.Name,
            b.Address,
            b.PhoneNumber,
            b.IsActive
        )).ToList();
    }
}
