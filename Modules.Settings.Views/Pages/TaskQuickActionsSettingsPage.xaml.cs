using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TaskQuickActionsSettingsPage.xaml
/// </summary>
public partial class TaskQuickActionsSettingsPage : GenericBasePage<TaskQuickActionsSettingsPageViewModel>, INotifyPropertyChanged
{
    public TaskQuickActionsSettingsPage(TaskQuickActionsSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
