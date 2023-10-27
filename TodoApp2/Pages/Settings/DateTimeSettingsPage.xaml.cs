using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for DateTimeSettingsPage.xaml
    /// </summary>
    public partial class DateTimeSettingsPage : BasePage<DateTimeSettingsPageViewModel>, INotifyPropertyChanged
    {
        public DateTimeSettingsPage(DateTimeSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
