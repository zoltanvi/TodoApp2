using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TaskItemSettingsPage.xaml
/// </summary>
public partial class TaskItemSettingsPage : GenericBasePage<TaskItemSettingsPageViewModel>, INotifyPropertyChanged
{
    public TaskItemSettingsPage(TaskItemSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
