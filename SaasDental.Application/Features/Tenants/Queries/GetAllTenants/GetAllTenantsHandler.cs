using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Tenants.Queries.GetAllTenants;

public class GetAllTenantsHandler : IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetAllTenantsHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<IEnumerable<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);

        return tenants.Select(t => new TenantDto(
            t.Id,
            t.Name,
            t.Address,
            t.IsActive,
            t.CreatedAt
        ));
    }
}
