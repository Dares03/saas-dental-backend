using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Inventory.Commands.CreateProduct;
using SaasDental.Application.Features.Inventory.Commands.RegisterInventoryMovement;
using SaasDental.Application.Features.Inventory.Queries.GetInventoryItems;
using SaasDental.Application.Features.Inventory.Queries.GetLowStockAlerts;
using SaasDental.Application.Features.Inventory.Queries.GetProductCategories;

namespace SaasDental.Api.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory")
            .RequireAuthorization()
            .WithTags("Inventory");

        // -- Listar Inventario por Sede --
        group.MapGet("/{branchId:guid}", async (Guid branchId, IMediator mediator) =>
        {
            var query = new GetInventoryItemsQuery(branchId);
            var results = await mediator.Send(query);
            return Results.Ok(results);
        })
        .Produces<List<InventoryItemDto>>()
        .Produces(StatusCodes.Status200OK);

        // -- Catálogo de Productos --
        group.MapPost("/products", async (IMediator mediator, [FromBody] CreateProductCommand command) =>
        {
            var productId = await mediator.Send(command);
            return Results.Created($"/api/inventory/products/{productId}", new { id = productId });
        })
        .Produces(StatusCodes.Status201Created);

        // -- Categorías de Producto --
        group.MapGet("/categories", async (IMediator mediator) =>
        {
            var query = new GetProductCategoriesQuery();
            var results = await mediator.Send(query);
            return Results.Ok(results);
        })
        .Produces<List<ProductCategoryDto>>()
        .Produces(StatusCodes.Status200OK);

        // -- Movimientos de Kardex --
        group.MapPost("/movements", async (IMediator mediator, [FromBody] RegisterInventoryMovementCommand command) =>
        {
            var movementId = await mediator.Send(command);
            return Results.Created($"/api/inventory/movements/{movementId}", new { id = movementId });
        })
        .Produces(StatusCodes.Status201Created);

        // -- Alertas de Stock --
        group.MapGet("/alerts/{branchId:guid}", async (Guid branchId, IMediator mediator) =>
        {
            var query = new GetLowStockAlertsQuery(branchId);
            var results = await mediator.Send(query);
            return Results.Ok(results);
        })
        .Produces<List<LowStockAlertDto>>()
        .Produces(StatusCodes.Status200OK);
    }
}

