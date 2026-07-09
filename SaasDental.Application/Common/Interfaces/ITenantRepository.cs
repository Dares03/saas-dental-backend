using SaasDental.Domain.Entities;

namespace SaasDental.Application.Common.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<Branch?> GetBranchByIdAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task AddBranchAsync(Branch branch, CancellationToken cancellationToken = default);
}
