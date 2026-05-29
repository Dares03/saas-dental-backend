using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Clinical.Commands.UpdateClinicalHistory;
using SaasDental.Application.Features.Clinical.Commands.CreateOdontogram;
using SaasDental.Application.Features.Clinical.Commands.AddFindingToOdontogram;
using SaasDental.Application.Features.Clinical.Queries.GetOdontogram;
using SaasDental.Application.Features.Clinical.Queries.GetHistoryByPatientId;

namespace SaasDental.Api.Endpoints;

public static class ClinicalEndpoints
{
    public static void MapClinicalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clinical")
            .RequireAuthorization()
            .WithTags("Clinical");

        // -- Clinical History --
        group.MapPut("/history", async (IMediator mediator, [FromBody] UpdateClinicalHistoryCommand command) =>
        {
            var historyId = await mediator.Send(command);
            return Results.Ok(new { id = historyId });
        })
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/history/patient/{patientId:guid}", async (Guid patientId, IMediator mediator) =>
        {
            var query = new GetHistoryByPatientIdQuery(patientId);
            var result = await mediator.Send(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .Produces<ClinicalHistoryDto>()
        .Produces(StatusCodes.Status404NotFound);

        // -- Odontogram --
        group.MapPost("/odontogram", async (IMediator mediator, [FromBody] CreateOdontogramCommand command) =>
        {
            var odontogramId = await mediator.Send(command);
            return Results.Created($"/api/clinical/odontogram/{odontogramId}", new { id = odontogramId });
        })
        .Produces(StatusCodes.Status201Created);

        group.MapGet("/odontogram/history/{clinicalHistoryId:guid}", async (Guid clinicalHistoryId, IMediator mediator) =>
        {
            var query = new GetOdontogramQuery(clinicalHistoryId);
            var result = await mediator.Send(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .Produces<OdontogramDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/odontogram/finding", async (IMediator mediator, [FromBody] AddFindingToOdontogramCommand command) =>
        {
            var findingId = await mediator.Send(command);
            return Results.Created($"/api/clinical/odontogram/finding/{findingId}", new { id = findingId });
        })
        .Produces(StatusCodes.Status201Created);
    }
}
