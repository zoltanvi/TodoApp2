using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel _AppViewModel;

        public ApplicationPage ActiveSettingsPage { get; set; } = ApplicationPage.WindowSettings;

        public ICommand GoBackCommand { get; }
        public ICommand SwitchToPageCommand { get; }

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            _AppViewModel = applicationViewModel;
            GoBackCommand = new RelayCommand(() => _AppViewModel.UpdateMainPage());

            SwitchToPageCommand = new RelayParameterizedCommand(SwitchToPage);
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
