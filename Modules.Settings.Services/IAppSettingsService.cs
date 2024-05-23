using Modules.Settings.ViewModels;

namespace Modules.Settings.Services;

public interface IAppSettingsService
{
    void UpdateAppSettingsFromDatabase(AppSettings appSettings);
    void UpdateDatabaseFromAppSettings(AppSettings appSettings);
}