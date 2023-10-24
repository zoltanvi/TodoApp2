using System.Windows.Input;

namespace TodoApp2.Core
{
    public class ThemeSettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel _appViewModel;
        public ICommand ChangeThemeCommand { get; }

        public ThemeSettingsPageViewModel()
        {
        }

        public ThemeSettingsPageViewModel(AppViewModel appViewModel)
        {
            _appViewModel = appViewModel;
            ChangeThemeCommand = new RelayParameterizedCommand(ChangeTheme);
        }

        private void ChangeTheme(object obj)
        {
            if (obj is ThemeViewModel themeViewModel)
            {
                //_appViewModel.ApplicationSettings.ActiveTheme = themeViewModel.Theme;
            }
        }
    }
}
