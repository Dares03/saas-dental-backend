using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Patients.Commands.CreatePatient;
using SaasDental.Application.Features.Patients.Commands.UpdatePatient;
using SaasDental.Application.Features.Patients.Queries.GetPatients;
using SaasDental.Application.Features.Patients.Queries.GetPatientById;

namespace SaasDental.Api.Endpoints;

public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients")
            .RequireAuthorization()
            .WithTags("Patients");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetPatientsQuery());
            return Results.Ok(result);
        })
        .Produces<List<PatientDto>>();

        group.MapGet("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var result = await mediator.Send(new GetPatientByIdQuery(id));
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .Produces<PatientDetailDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (IMediator mediator, [FromBody] CreatePatientCommand command) =>
        {
            try
            {
                var patientId = await mediator.Send(command);
                return Results.Created($"/api/patients/{patientId}", new { id = patientId });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .Produces(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", async (IMediator mediator, Guid id, [FromBody] UpdatePatientCommand command) =>
        {
            var updated = await mediator.Send(command with { PatientId = id });
            return Results.Ok(new { success = updated });
        })
        .Produces(StatusCodes.Status200OK);
    }
}
