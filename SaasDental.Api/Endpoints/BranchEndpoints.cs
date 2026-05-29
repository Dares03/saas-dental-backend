using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Branches.Commands.CreateBranch;
using SaasDental.Application.Features.Branches.Commands.UpdateBranch;
using SaasDental.Application.Features.Branches.Queries.GetBranches;

namespace SaasDental.Api.Endpoints;

public static class BranchEndpoints
{
    public static void MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/branches")
            .RequireAuthorization()
            .WithTags("Branches");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBranchesQuery());
            return Results.Ok(result);
        })
        .Produces<List<BranchDto>>();

        group.MapPost("/", async (IMediator mediator, [FromBody] CreateBranchCommand command) =>
        {
            var branchId = await mediator.Send(command);
            return Results.Created($"/api/branches/{branchId}", new { id = branchId });
        })
        .Produces(StatusCodes.Status201Created)
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPut("/{id:guid}", async (Guid id, IMediator mediator, [FromBody] UpdateBranchRequest request) =>
        {
            var command = new UpdateBranchCommand(id, request.Name, request.Address, request.PhoneNumber);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}

public record UpdateBranchRequest(string Name, string Address, string PhoneNumber);
