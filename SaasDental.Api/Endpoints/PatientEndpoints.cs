using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Patients.Commands.CreatePatient;
using SaasDental.Application.Features.Patients.Queries.GetPatients;

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

        group.MapPost("/", async (IMediator mediator, [FromBody] CreatePatientCommand command) =>
        {
            var patientId = await mediator.Send(command);
            return Results.Created($"/api/patients/{patientId}", new { id = patientId });
        })
        .Produces(StatusCodes.Status201Created);
    }
}
