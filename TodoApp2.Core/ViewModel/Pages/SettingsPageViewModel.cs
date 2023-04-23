using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel m_ApplicationViewModel;

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            m_ApplicationViewModel = applicationViewModel;
        }
    }
}
