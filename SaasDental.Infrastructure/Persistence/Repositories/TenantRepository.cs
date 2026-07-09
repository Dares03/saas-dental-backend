using Microsoft.EntityFrameworkCore;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of ITenantRepository.
/// Bypasses the Global Query Filter since Tenants are not tenant-scoped themselves.
/// </summary>
public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AnyAsync(t => t.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Branch?> GetBranchByIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Branch>().FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
    }

    public async Task AddBranchAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await _context.Set<Branch>().AddAsync(branch, cancellationToken);
    }
}
