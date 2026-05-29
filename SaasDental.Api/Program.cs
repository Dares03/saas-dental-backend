using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SaasDental.Application;
using SaasDental.Infrastructure;
using SaasDental.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ── Application & Infrastructure ─────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.Configure<SaasDental.Application.Common.Settings.JwtSettings>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();  // Required by HttpContextTenantService

// ── JWT Authentication ────────────────────────────────────────
var jwtSecret   = builder.Configuration["Jwt:Secret"]!;
var jwtIssuer   = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer   = true,
            ValidIssuer      = jwtIssuer,
            ValidateAudience = true,
            ValidAudience    = jwtAudience,
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ── CORS (allow frontend dev server) ─────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── Swagger with JWT support ──────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "SaasDental API",
        Version     = "v1",
        Description = "API multitenant para gestión de clínicas dentales."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Pega tu token JWT aquí. No incluyas 'Bearer ', solo el token."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// ── Build ─────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SaasDental API v1"));
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

// ORDER MATTERS: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

// ── Routes ────────────────────────────────────────────────────
app.MapGet("/", () => Results.Ok(new { status = "running", api = "SaasDental API v1" }));
app.MapAuthEndpoints();
app.MapTenantEndpoints();
app.MapBranchEndpoints();
app.MapUserEndpoints();
app.MapPatientEndpoints();
app.MapAppointmentEndpoints();
app.MapClinicalEndpoints();
app.MapFinancialEndpoints();
app.MapInventoryEndpoints();

app.Run();
