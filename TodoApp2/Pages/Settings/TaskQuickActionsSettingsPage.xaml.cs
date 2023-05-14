using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskQuickActionsSettingsPage.xaml
    /// </summary>
    public partial class TaskQuickActionsSettingsPage : BasePage<TaskQuickActionsSettingsPageViewModel>, INotifyPropertyChanged
    {
        public TaskQuickActionsSettingsPage(TaskQuickActionsSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
