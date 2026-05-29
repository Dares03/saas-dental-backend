using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Financial.Commands.CreateTreatmentService;
using SaasDental.Application.Features.Financial.Commands.OpenCashRegister;
using SaasDental.Application.Features.Financial.Commands.AddIncome;

namespace SaasDental.Api.Endpoints;

public static class FinancialEndpoints
{
    public static void MapFinancialEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/financial")
            .RequireAuthorization()
            .WithTags("Financial");

        // -- Catálogo de Servicios --
        group.MapPost("/services", async (IMediator mediator, [FromBody] CreateTreatmentServiceCommand command) =>
        {
            var serviceId = await mediator.Send(command);
            return Results.Created($"/api/financial/services/{serviceId}", new { id = serviceId });
        })
        .Produces(StatusCodes.Status201Created);

        // -- Caja (Cash Register) --
        group.MapPost("/cash-register/open", async (IMediator mediator, [FromBody] OpenCashRegisterCommand command) =>
        {
            var cashRegisterId = await mediator.Send(command);
            return Results.Created($"/api/financial/cash-register/{cashRegisterId}", new { id = cashRegisterId });
        })
        .Produces(StatusCodes.Status201Created);

        // -- Transacciones --
        group.MapPost("/cash-register/income", async (IMediator mediator, [FromBody] AddIncomeCommand command) =>
        {
            var transactionId = await mediator.Send(command);
            return Results.Created($"/api/financial/transactions/{transactionId}", new { id = transactionId });
        })
        .Produces(StatusCodes.Status201Created);
    }
}
