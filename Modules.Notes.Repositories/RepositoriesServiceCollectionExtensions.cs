using Microsoft.Extensions.DependencyInjection;

namespace Modules.Notes.Repositories;

public static class RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddNotesRepositories(this IServiceCollection services)
    {
        services.AddDbContext<NotesDbContext>();
        services.AddScoped<INotesRepository, NotesRepository>();
        services.AddScoped<INotesDbInfoRepository, NotesDbInfoRepository>();

        return services;
    }
}
