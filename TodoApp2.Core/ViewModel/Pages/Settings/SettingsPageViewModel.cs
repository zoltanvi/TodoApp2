using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel _appViewModel;

        public ApplicationPage ActiveSettingsPage { get; set; } = ApplicationPage.ApplicationSettings;

        public ICommand GoBackCommand { get; }
        public ICommand SwitchToPageCommand { get; }

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            _appViewModel = applicationViewModel;
            GoBackCommand = new RelayCommand(() => _appViewModel.UpdateMainPage());

            SwitchToPageCommand = new RelayParameterizedCommand<object>(SwitchToPage);
        }

        private void SwitchToPage(object obj)
        {
            if (obj is ApplicationPage page)
            {
                ActiveSettingsPage = page;
            }
        }
    }
}
