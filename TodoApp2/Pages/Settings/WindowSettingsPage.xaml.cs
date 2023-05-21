using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for WindowSettingsPage.xaml
    /// </summary>
    public partial class WindowSettingsPage : BasePage<WindowSettingsPageViewModel>, INotifyPropertyChanged
    {
        public WindowSettingsPage(WindowSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
