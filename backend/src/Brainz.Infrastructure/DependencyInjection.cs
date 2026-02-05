using Brainz.Application.Interfaces;
using Brainz.Domain.Interfaces;
using Brainz.Infrastructure.Data;
using Brainz.Infrastructure.Repositories;
using Brainz.Infrastructure.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brainz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BrainzDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(BrainzDbContext).Assembly.FullName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ISyncLogRepository, SyncLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
        services.AddHangfireServer();

        return services;
    }
}
