using Brainz.Application.Interfaces;
using Brainz.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Brainz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
