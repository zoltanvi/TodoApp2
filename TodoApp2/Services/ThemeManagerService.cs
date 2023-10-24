using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    public class ThemeManagerService : IThemeManagerService
    {
        private static IThemeManagerService _instance;
        private AppSettings _appSettings;

        internal static IThemeManagerService Get(AppSettings appSettings) => _instance ?? (_instance = new ThemeManagerService(appSettings));

        private ThemeManagerService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _appSettings.ThemeSettings.PropertyChanged += ThemeSettings_PropertyChanged;
        }

        private void ThemeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(AppSettings.ThemeSettings.SeedColor)) return;

            OnThemeSeedChanged();
        }

        private void OnThemeSeedChanged()
        {
            
        }
    }
}
