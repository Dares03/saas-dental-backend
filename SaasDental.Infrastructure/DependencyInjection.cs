using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Infrastructure.Persistence;
using SaasDental.Infrastructure.Persistence.Repositories;

namespace SaasDental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Register repositories (Infrastructure adapters)
        services.AddScoped<ITenantRepository, TenantRepository>();

        return services;
    }
}
