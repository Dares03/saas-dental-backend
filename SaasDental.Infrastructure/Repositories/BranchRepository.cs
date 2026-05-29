using Microsoft.EntityFrameworkCore;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Infrastructure.Persistence;

namespace SaasDental.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BranchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Branch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Global query filter handles TenantId isolation
        return await _dbContext.Branches
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Global query filter handles TenantId isolation
        return await _dbContext.Branches
            .Where(b => b.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await _dbContext.Branches.AddAsync(branch, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        _dbContext.Branches.Update(branch);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
