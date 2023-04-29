using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel m_ApplicationViewModel;

        public ICommand ChangeThemeCommand { get; }

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            m_ApplicationViewModel = applicationViewModel;
            ChangeThemeCommand = new RelayParameterizedCommand(ChangeTheme);
        }

        private void ChangeTheme(object obj)
        {
            if (obj is ThemeViewModel themeViewModel)
            {
                m_ApplicationViewModel.ApplicationSettings.ActiveTheme = themeViewModel.Theme;
            }
        }
    }
}
