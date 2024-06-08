using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Database;
using Modules.Common.Navigation;
using Modules.Common.Services.Navigation;
using Modules.Common.Views.Services.Navigation;
using Modules.Notes.Repositories;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Repositories;
using Modules.Settings.Services;
using Modules.Settings.Views;
using Modules.Settings.Views.Pages;
using TodoApp2.Core;
using TodoApp2.DefaultData;
using TodoApp2.Persistence;
using TodoApp2.Services;
using TodoApp2.WindowHandling;

namespace TodoApp2;

public static class Program
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
    {
        services.AddScoped<MainWindow>();
        services.AddScoped<IAppContext, Persistence.AppContext>(provider =>
        {
            return new Persistence.AppContext(DbConfiguration.ConnectionStringOld);
        });
        services.AddScoped<IUIScaler, UIScaler>();

        services.AddScoped<AppViewModel>();
        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<IWindowService, WindowService>();
        services.AddSingleton<IThemeManagerService, MaterialThemeManagerService>();
        services.AddSingleton<AppSettings>(provider =>
        {
            return AppSettings.Instance;
        });
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddScoped<UndoManager>();
        services.AddScoped<MediaPlayerService>();
        services.AddScoped<ZoomingListener>();
        services.AddSingleton<UIScaler>(provider =>
        {
            return UIScaler.Instance;
        });
        services.AddSingleton<IUIScaler>(provider =>
        {
            return UIScaler.Instance;
        });
        services.AddScoped<ThemeChangeNotifier>();
        services.AddScoped<OverlayPageService>();

        services.AddScoped<CategoryListService>();
        services.AddScoped<TaskListService>();
        services.AddScoped<NoteListService>();
        services.AddScoped<TaskScheduler2>();
        services.AddScoped<ReminderNotificationService>();
        services.AddScoped<OneEditorOpenService>();


        services.AddScoped<SettingsPage>();
        services.AddScoped<SettingsPageViewModel>();
        services.AddSettingsViews();

        services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>();
        services.AddSingleton<ISideMenuPageNavigationService, SideMenuPageNavigationService>();
        services.AddSingleton<IOverlayPageNavigationService, OverlayPageNavigationService>();

        AddDatabases(services);
        AddPages(services);

        return services;
    }

    private static void AddDatabases(IServiceCollection services)
    {
        services.AddScoped<DefaultDataCreator>();

        services.AddSettingsRepositories();
        services.AddNotesRepositories();

        services.AddMigrationsService();
    }

    private static void AddPages(IServiceCollection services)
    {
        services.AddScoped<ICategoryListPage, CategoryPage>();
        services.AddScoped<INoteEditorPage, NotePage>();
        services.AddScoped<INoteListPage, NoteListPage>();
        services.AddScoped<IRecycleBinPage, RecycleBinPage>();
        services.AddScoped<ISettingsPage, SettingsPage>();
        services.AddScoped<ITaskPage, TaskPage>();

        services.AddScoped<CategoryPageViewModel>();
        services.AddScoped<NotePageViewModel>();
        services.AddScoped<NoteListPageViewModel>();
        services.AddScoped<RecycleBinPageViewModel>();
        services.AddScoped<SettingsPageViewModel>();
        services.AddScoped<TaskPageViewModel>();

        services.AddSingleton<MessageService>();
    }
}
