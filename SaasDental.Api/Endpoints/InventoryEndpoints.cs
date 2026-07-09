using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaasDental.Application.Features.Inventory.Commands.ArchiveProductCategory;
using SaasDental.Application.Features.Inventory.Commands.UnarchiveProductCategory;
using SaasDental.Application.Features.Inventory.Commands.CreateProduct;
using SaasDental.Application.Features.Inventory.Commands.CreateProductCategory;
using SaasDental.Application.Features.Inventory.Commands.DeleteProduct;
using SaasDental.Application.Features.Inventory.Commands.DeleteProductCategory;
using SaasDental.Application.Features.Inventory.Commands.RegisterInventoryMovement;
using SaasDental.Application.Features.Inventory.Queries.GetInventoryItems;
using SaasDental.Application.Features.Inventory.Queries.GetLowStockAlerts;
using SaasDental.Application.Features.Inventory.Queries.GetProductCategories;

namespace SaasDental.Api.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/inventory/debug", async (SaasDental.Infrastructure.Persistence.ApplicationDbContext db) =>
        {
            var branches = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(db.Branches);
            var products = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(db.Products);
            var inventoryItems = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(db.InventoryItems);
            return Results.Ok(new { branches, products, inventoryItems });
        });

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

        group.MapDelete("/products/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteProductCommand(id));
            return Results.Ok(new { success = result });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // -- Categorías de Producto --
        group.MapGet("/categories", async (IMediator mediator) =>
        {
            var query = new GetProductCategoriesQuery();
            var results = await mediator.Send(query);
            return Results.Ok(results);
        })
        .Produces<List<ProductCategoryDto>>()
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/categories", async (IMediator mediator, [FromBody] CreateProductCategoryCommand command) =>
        {
            var categoryId = await mediator.Send(command);
            return Results.Created($"/api/inventory/categories/{categoryId}", new { id = categoryId });
        })
        .Produces(StatusCodes.Status201Created);

        group.MapPut("/categories/{id:guid}/archive", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new ArchiveProductCategoryCommand(id));
            return Results.Ok(new { success = result });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/categories/{id:guid}/unarchive", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new UnarchiveProductCategoryCommand(id));
            return Results.Ok(new { success = result });
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/categories/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(new DeleteProductCategoryCommand(id));
                return Results.Ok(new { success = result });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { detail = ex.Message });
            }
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // -- Movimientos de Kardex --
        group.MapGet("/movements/{productId:guid}/{branchId:guid}", async (Guid productId, Guid branchId, IMediator mediator) =>
        {
            var query = new SaasDental.Application.Features.Inventory.Queries.GetInventoryMovements.GetInventoryMovementsQuery(productId, branchId);
            var results = await mediator.Send(query);
            return Results.Ok(results);
        })
        .Produces<List<SaasDental.Application.Features.Inventory.Queries.GetInventoryMovements.InventoryMovementDto>>()
        .Produces(StatusCodes.Status200OK);

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

