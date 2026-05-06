using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamFlow.Application.Interfaces;
using TeamFlow.Infrastructure.Identity;
using TeamFlow.Infrastructure.Logging;
using TeamFlow.Infrastructure.Persistence.Data;
using TeamFlow.Infrastructure.Persistence.UnitOfWork;
using TeamFlow.Domain.Interfaces;

namespace TeamFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<JwtSettings>(options =>
        {
            options.Issuer = configuration["JwtSettings:Issuer"]!;
            options.Audience = configuration["JwtSettings:Audience"]!;
            options.Secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
        });

        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        //e3
        return services;
    }
}