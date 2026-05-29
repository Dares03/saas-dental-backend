using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Branches.Commands.UpdateBranch;

public record UpdateBranchCommand(Guid Id, string Name, string Address, string PhoneNumber) : IRequest;

public class UpdateBranchValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}

public class UpdateBranchHandler : IRequestHandler<UpdateBranchCommand>
{
    private readonly IBranchRepository _branchRepository;

    public UpdateBranchHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdAsync(request.Id, cancellationToken);

        if (branch == null)
            throw new Exception("Sede no encontrada."); // Consider throwing a specific NotFoundException

        branch.UpdateDetails(request.Name, request.Address, request.PhoneNumber);

        await _branchRepository.UpdateAsync(branch, cancellationToken);
    }
}
