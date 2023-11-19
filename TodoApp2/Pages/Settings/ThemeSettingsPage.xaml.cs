using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for ThemeSettingsPage.xaml
    /// </summary>
    public partial class ThemeSettingsPage : BasePage<ThemeSettingsPageViewModel>, INotifyPropertyChanged
    {
        public ThemeSettingsPage(ThemeSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
