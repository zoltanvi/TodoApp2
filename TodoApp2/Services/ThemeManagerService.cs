using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    public class ThemeManagerService : IThemeManagerService
    {
        private static IThemeManagerService _instance;
        private ApplicationSettings _appSettings;

        internal static IThemeManagerService Get(ApplicationSettings appSettings) => _instance ?? (_instance = new ThemeManagerService(appSettings));

        private ThemeManagerService(ApplicationSettings applicationSettings)
        {
            _appSettings = applicationSettings;
            _appSettings.PropertyChanged += OnAppSettingChanged;
        }

        private void OnThemeSeedChanged()
        {
            
        }

        private void OnAppSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ApplicationSettings.ThemeSeed)) return;

            OnThemeSeedChanged();
        }
    }
}
