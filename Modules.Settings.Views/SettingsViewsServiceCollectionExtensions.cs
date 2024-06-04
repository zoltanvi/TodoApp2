using Microsoft.Extensions.DependencyInjection;
using Modules.Settings.Views.Pages;
using System.Windows;

namespace Modules.Settings.Views;

public static class SettingsViewsServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsViews(this IServiceCollection services)
    {
        services.AddScoped<NotePageSettingsPageViewModel>();
        services.AddScoped<TaskItemSettingsPageViewModel>();
        services.AddScoped<TaskPageSettingsPageViewModel>();
        services.AddScoped<TaskQuickActionsSettingsPageViewModel>();
        services.AddScoped<TextEditorQuickActionsSettingsPageViewModel>();
        services.AddScoped<ThemeSettingsPageViewModel>();
        services.AddScoped<ApplicationSettingsPageViewModel>();
        services.AddScoped<PageTitleSettingsPageViewModel>();
        services.AddScoped<DateTimeSettingsPageViewModel>();
        services.AddScoped<ShortcutsPageViewModel>();

        services.AddScoped<NotePageSettingsPage>();
        services.AddScoped<TaskItemSettingsPage>();
        services.AddScoped<TaskPageSettingsPage>();
        services.AddScoped<TaskQuickActionsSettingsPage>();
        services.AddScoped<TextEditorQuickActionsSettingsPage>();
        services.AddScoped<ThemeSettingsPage>();
        services.AddScoped<ApplicationSettingsPage>();
        services.AddScoped<PageTitleSettingsPage>();
        services.AddScoped<DateTimeSettingsPage>();
        services.AddScoped<ShortcutsPage>();

        return services;
    }
}
