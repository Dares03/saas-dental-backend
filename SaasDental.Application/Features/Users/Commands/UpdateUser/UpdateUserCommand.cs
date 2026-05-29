using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, Guid? DefaultBranchId) : IRequest;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
            throw new Exception("Usuario no encontrado."); // TODO: Switch to custom NotFoundException

        user.UpdateProfile(request.FirstName, request.LastName);
        user.SetDefaultBranch(request.DefaultBranchId);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
