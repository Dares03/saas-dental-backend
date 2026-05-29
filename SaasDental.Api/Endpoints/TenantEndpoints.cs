using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Tenants.Commands.CreateTenant;
using SaasDental.Application.Features.Tenants.Queries.GetAllTenants;
using SaasDental.Application.Features.Tenants.Queries.GetTenantById;

namespace SaasDental.Api.Endpoints;

public static class TenantEndpoints
{
    public static void MapTenantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tenants")
                       .WithTags("Tenants");

        // GET /api/tenants
        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var tenants = await mediator.Send(new GetAllTenantsQuery(), ct);
            return Results.Ok(tenants);
        })
        .WithName("GetAllTenants")
        .WithSummary("Obtiene todas las clínicas registradas en el sistema.");

        // GET /api/tenants/{id}
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var tenant = await mediator.Send(new GetTenantByIdQuery(id), ct);
            return tenant is null
                ? Results.NotFound(new { message = $"No se encontró una clínica con el Id '{id}'." })
                : Results.Ok(tenant);
        })
        .WithName("GetTenantById")
        .WithSummary("Obtiene el detalle de una clínica por su Id.");

        // POST /api/tenants
        group.MapPost("/", async ([FromBody] CreateTenantCommand command, IMediator mediator, CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.Send(command, ct);
                return Results.Created($"/api/tenants/{result.Id}", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        })
        .WithName("CreateTenant")
        .WithSummary("Registra una nueva clínica (Tenant) en el sistema.");
    }
}
