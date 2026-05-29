using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Application.Common.Settings;
using SaasDental.Infrastructure.Persistence;
using SaasDental.Infrastructure.Persistence.Repositories;
using SaasDental.Infrastructure.Repositories;
using SaasDental.Infrastructure.Security;
using SaasDental.Infrastructure.Services;

namespace SaasDental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Database ──────────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // ── Repositories (Hexagonal Adapters) ─────────────────────
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IClinicalRepository, ClinicalRepository>();
        services.AddScoped<IFinancialRepository, FinancialRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        // ── Security services ─────────────────────────────────────
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // ── Tenant context (reads claim from JWT via IHttpContextAccessor) ──
        services.AddScoped<ITenantService, HttpContextTenantService>();

        return services;
    }
}
