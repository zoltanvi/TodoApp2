using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskItemSettingsPage.xaml
    /// </summary>
    public partial class TaskItemSettingsPage : BasePage<TaskItemSettingsPageViewModel>, INotifyPropertyChanged
    {
        public TaskItemSettingsPage(TaskItemSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
