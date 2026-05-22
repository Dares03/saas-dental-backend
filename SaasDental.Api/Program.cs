using SaasDental.Application;
using SaasDental.Infrastructure;
using SaasDental.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Mock ITenantService for now until Auth is implemented
builder.Services.AddScoped<ITenantService, MockTenantService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Basic Minimal API endpoint
app.MapGet("/", () => "SaasDental API is running!");

app.Run();

// Temporary mock service
public class MockTenantService : ITenantService
{
    public Guid? GetCurrentTenantId()
    {
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
