using Microsoft.Extensions.DependencyInjection;

namespace Modules.Settings.Reg;

public static class SettingsServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddDbContext<SettingDbContext>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();

        return services;
    }
}
