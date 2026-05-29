using FluentValidation;
using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid Id) : IRequest;

public class DeactivateUserValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
            throw new Exception("Usuario no encontrado.");

        // We need a deactivate method in the User entity
        user.Deactivate();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
