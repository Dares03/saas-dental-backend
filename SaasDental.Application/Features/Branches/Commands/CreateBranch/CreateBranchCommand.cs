using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Branches.Commands.CreateBranch;

public record CreateBranchCommand(string Name, string Address, string PhoneNumber) : IRequest<Guid>;

public class CreateBranchValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}

public class CreateBranchHandler : IRequestHandler<CreateBranchCommand, Guid>
{
    private readonly IBranchRepository _branchRepository;
    private readonly ITenantService _tenantService;

    public CreateBranchHandler(IBranchRepository branchRepository, ITenantService tenantService)
    {
        _branchRepository = branchRepository;
        _tenantService = tenantService;
    }

    public async Task<Guid> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId() 
            ?? throw new UnauthorizedAccessException("El contexto no tiene un Tenant válido.");

        var branch = new Branch(request.Name, request.Address, request.PhoneNumber, tenantId);

        await _branchRepository.AddAsync(branch, cancellationToken);

        return branch.Id;
    }
}
