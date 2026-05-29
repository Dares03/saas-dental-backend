using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Appointments.Commands.ScheduleAppointment;
using SaasDental.Application.Features.Appointments.Commands.UpdateAppointmentStatus;
using SaasDental.Application.Features.Appointments.Queries.GetAppointmentsByBranch;
using SaasDental.Domain.Enums;

namespace SaasDental.Api.Endpoints;

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments")
            .RequireAuthorization()
            .WithTags("Appointments");

        group.MapGet("/branch/{branchId:guid}", async (Guid branchId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, IMediator mediator) =>
        {
            var query = new GetAppointmentsByBranchQuery(branchId, startDate, endDate);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .Produces<List<AppointmentDto>>();

        group.MapPost("/", async (IMediator mediator, [FromBody] ScheduleAppointmentCommand command) =>
        {
            var appointmentId = await mediator.Send(command);
            return Results.Created($"/api/appointments/{appointmentId}", new { id = appointmentId });
        })
        .Produces(StatusCodes.Status201Created);

        group.MapPatch("/{id:guid}/status", async (Guid id, [FromBody] UpdateAppointmentStatusRequest request, IMediator mediator) =>
        {
            var command = new UpdateAppointmentStatusCommand(id, request.NewStatus);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent);
    }
}

public record UpdateAppointmentStatusRequest(AppointmentStatus NewStatus);
