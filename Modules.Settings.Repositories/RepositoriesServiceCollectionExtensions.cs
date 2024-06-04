using Microsoft.Extensions.DependencyInjection;
using Modules.Settings.Contracts;

namespace Modules.Settings.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsRepositories(this IServiceCollection services)
    {
        services.AddDbContext<SettingDbContext>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();

        return services;
    }
}
