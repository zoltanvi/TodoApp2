using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : BasePage<SettingsPageViewModel>
    {
        public SettingsPage(SettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}