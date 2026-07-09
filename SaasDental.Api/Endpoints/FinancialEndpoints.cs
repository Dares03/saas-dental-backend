using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Financial.Commands.CreateTreatmentService;
using SaasDental.Application.Features.Financial.Commands.OpenCashRegister;
using SaasDental.Application.Features.Financial.Commands.CloseCashRegister;
using SaasDental.Application.Features.Financial.Commands.AddIncome;
using SaasDental.Application.Features.Financial.Commands.AddExpense;
using SaasDental.Application.Features.Financial.Queries.GetActiveCashRegister;
using SaasDental.Application.Features.Financial.Queries.GetCashRegistersHistory;
using SaasDental.Application.Features.Financial.Queries.GetCashRegisterTransactions;

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

        group.MapPost("/cash-register/close", async (IMediator mediator, [FromBody] CloseCashRegisterCommand command) =>
        {
            await mediator.Send(command);
            return Results.Ok();
        })
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/cash-register/active/{branchId:guid}", async (Guid branchId, IMediator mediator) =>
        {
            var query = new GetActiveCashRegisterQuery(branchId);
            var result = await mediator.Send(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .Produces<CashRegisterDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/cash-register/history/{branchId:guid}", async (Guid branchId, IMediator mediator) =>
        {
            var query = new GetCashRegistersHistoryQuery(branchId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .Produces<List<CashRegisterHistoryDto>>()
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/cash-register/{cashRegisterId:guid}/transactions", async (Guid cashRegisterId, IMediator mediator) =>
        {
            var query = new GetCashRegisterTransactionsQuery(cashRegisterId);
            var results = await mediator.Send(query);
            return Results.Ok(results);
        })
        .Produces<List<CashTransactionDto>>()
        .Produces(StatusCodes.Status200OK);

        // -- Transacciones --
        group.MapPost("/cash-register/income", async (IMediator mediator, [FromBody] AddIncomeCommand command) =>
        {
            var transactionId = await mediator.Send(command);
            return Results.Created($"/api/financial/transactions/{transactionId}", new { id = transactionId });
        })
        .Produces(StatusCodes.Status201Created);

        group.MapPost("/cash-register/expense", async (IMediator mediator, [FromBody] AddExpenseCommand command) =>
        {
            var transactionId = await mediator.Send(command);
            return Results.Created($"/api/financial/transactions/{transactionId}", new { id = transactionId });
        })
        .Produces(StatusCodes.Status201Created);
    }
}

