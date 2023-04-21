using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel m_ApplicationViewModel;

        public ICommand BackToPrevPageCommand { get; }

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            m_ApplicationViewModel = applicationViewModel;
            BackToPrevPageCommand = new RelayCommand(BackToPrevPage);
        }

        // TODO: Implement page handler for history if more than 2 page exist that is shown in the side panel
        private void BackToPrevPage()
        {
            m_ApplicationViewModel.SideMenuPage = ApplicationPage.Category;
        }
    }
}
