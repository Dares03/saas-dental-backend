using MediatR;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;

namespace SaasDental.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, CreateTenantResult>
{
    private readonly ITenantRepository _tenantRepository;

    public CreateTenantHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<CreateTenantResult> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Business rule: Name must be unique
        var alreadyExists = await _tenantRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (alreadyExists)
            throw new InvalidOperationException($"Ya existe una clínica registrada con el nombre '{request.Name}'.");

        // Create aggregate root via domain constructor (private setters preserved)
        var tenant = new Tenant(request.Name, request.Address);

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);

        return new CreateTenantResult(tenant.Id, tenant.Name, tenant.Address, tenant.IsActive);
    }
}
