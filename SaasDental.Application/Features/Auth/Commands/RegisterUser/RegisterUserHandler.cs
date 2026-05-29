using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify the target tenant exists and is active
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant is null || !tenant.IsActive)
            throw new InvalidOperationException($"No se encontró la clínica con Id '{request.TenantId}' o no está activa.");

        // 2. Email must be unique across the system
        if (await _userRepository.ExistsByEmailAsync(request.Email.ToLower(), cancellationToken))
            throw new InvalidOperationException($"Ya existe un usuario registrado con el correo '{request.Email}'.");

        // 3. Hash password and create domain entity
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email.ToLower(),
            passwordHash,
            request.Role,
            request.TenantId
        );

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new RegisterUserResult(user.Id, user.Email, user.Role, user.TenantId);
    }
}
