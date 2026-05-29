using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Users.Commands.UpdateUser;
using SaasDental.Application.Features.Users.Queries.GetUsers;

namespace SaasDental.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .RequireAuthorization()
            .WithTags("Users");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUsersQuery());
            return Results.Ok(result);
        })
        .Produces<List<UserDto>>();

        group.MapPut("/{id:guid}", async (Guid id, IMediator mediator, [FromBody] UpdateUserRequest request) =>
        {
            var command = new UpdateUserCommand(id, request.FirstName, request.LastName, request.DefaultBranchId);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var command = new SaasDental.Application.Features.Users.Commands.DeactivateUser.DeactivateUserCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}

public record UpdateUserRequest(string FirstName, string LastName, Guid? DefaultBranchId);
