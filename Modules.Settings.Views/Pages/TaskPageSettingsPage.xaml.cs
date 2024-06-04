using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TaskPageSettingsPage.xaml
/// </summary>
public partial class TaskPageSettingsPage : GenericBasePage<TaskPageSettingsPageViewModel>, INotifyPropertyChanged
{
    public TaskPageSettingsPage(TaskPageSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
