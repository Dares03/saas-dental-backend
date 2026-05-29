using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Auth.Commands.Login;
using SaasDental.Application.Features.Auth.Commands.RegisterUser;

namespace SaasDental.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // ── POST /api/auth/register ──────────────────────────────
        group.MapPost("/register", async (
            [FromBody] RegisterUserCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.Send(command, ct);
                return Results.Created($"/api/users/{result.Id}", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        })
        .WithName("RegisterUser")
        .WithSummary("Registra un nuevo usuario en una clínica. (Admin only en producción)");

        // ── POST /api/auth/login ─────────────────────────────────
        group.MapPost("/login", async (
            [FromBody] LoginCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.Send(command, ct);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Unauthorized(); // Never leak the reason
            }
        })
        .WithName("Login")
        .WithSummary("Autentica un usuario y retorna un JWT Bearer token.");
    }
}
