using SaasDental.Application;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Infrastructure;
using SaasDental.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ── Application & Infrastructure Services ───────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── Temporary Mock TenantService (replaced once JWT Auth is implemented) ─────
builder.Services.AddScoped<ITenantService, MockTenantService>();

// ── CORS (allow frontend dev server) ─────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "SaasDental API", 
        Version = "v1",
        Description = "API multitenant para gestión de clínicas dentales." 
    });
});

var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SaasDental API v1"));
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

// ── Minimal API routes ────────────────────────────────────────────────────────
app.MapGet("/", () => Results.Ok(new { status = "running", api = "SaasDental API v1" }));
app.MapTenantEndpoints();

app.Run();

// ── Temporary mock (removed once real JWT middleware is implemented) ──────────
public class MockTenantService : ITenantService
{
    public Guid? GetCurrentTenantId()
        => Guid.Parse("00000000-0000-0000-0000-000000000001");
}
