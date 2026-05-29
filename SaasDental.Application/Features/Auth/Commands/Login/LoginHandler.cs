using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace SaasDental.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public LoginHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IJwtTokenGenerator jwtGenerator,
        IPasswordHasher passwordHasher,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _jwtGenerator = jwtGenerator;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Find user by email (case-insensitive)
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLower(), cancellationToken);

        // 2. Generic error — never reveal if email exists or not
        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // 3. Get tenant name to embed in token
        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant is null || !tenant.IsActive)
            throw new UnauthorizedAccessException("La clínica asociada no está activa.");

        // 4. Generate JWT
        var token = _jwtGenerator.GenerateToken(user, tenant.Name);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new LoginResult(
            token,
            expiresAt,
            new AuthUserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role, user.TenantId, tenant.Name)
        );
    }
}
