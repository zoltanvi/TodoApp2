using Microsoft.Extensions.DependencyInjection;
using Modules.Migrations;

namespace Modules.Settings.Repositories;

public static class MigrationsServiceCollectionExtensions
{
    public static IServiceCollection AddMigrationsService(this IServiceCollection services)
    {
        services.AddScoped<IMigrationService, MigrationService>();

        return services;
    }
}
