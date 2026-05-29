using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Infrastructure.Security;

/// <summary>
/// BCrypt-based password hasher.
/// Work factor 12 is a good balance between security and performance (≈250ms on modern hardware).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword)
        => BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);

    public bool Verify(string plainPassword, string hash)
        => BCrypt.Net.BCrypt.Verify(plainPassword, hash);
}
