using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel m_AppViewModel;

        public ICommand ChangeThemeCommand { get; }
        public ICommand GoBackCommand { get; }

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            m_AppViewModel = applicationViewModel;
            ChangeThemeCommand = new RelayParameterizedCommand(ChangeTheme);
            GoBackCommand = new RelayCommand(() => { m_AppViewModel.UpdateMainPage(); });
        }

        private void ChangeTheme(object obj)
        {
            if (obj is ThemeViewModel themeViewModel)
            {
                m_AppViewModel.ApplicationSettings.ActiveTheme = themeViewModel.Theme;
            }
        }
    }
}
