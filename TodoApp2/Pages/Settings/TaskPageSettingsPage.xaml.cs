using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPageSettingsPage.xaml
    /// </summary>
    public partial class TaskPageSettingsPage : BasePage<TaskPageSettingsPageViewModel>, INotifyPropertyChanged
    {
        public TaskPageSettingsPage(TaskPageSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
