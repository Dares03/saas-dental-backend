using MediatR;
using SaasDental.Application.Common.Interfaces;

namespace SaasDental.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdHandler : IRequestHandler<GetTenantByIdQuery, TenantDetailDto?>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDetailDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (tenant is null)
            return null;

        return new TenantDetailDto(
            tenant.Id,
            tenant.Name,
            tenant.Address,
            tenant.IsActive,
            tenant.CreatedAt,
            tenant.Users.Count
        );
    }
}
