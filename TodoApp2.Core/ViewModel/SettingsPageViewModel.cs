using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private ApplicationViewModel Application => IoC.Application;

        public ICommand BackToPrevPageCommand { get; }

        public SettingsPageViewModel()
        {
            BackToPrevPageCommand = new RelayCommand(BackToPrevPage);
        }

        // TODO: Implement page handler for history if more than 2 page exist that is shown in the side panel
        private void BackToPrevPage()
        {
            Application.SideMenuPage = ApplicationPage.Category;
        }
    }
}
