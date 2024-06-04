using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for ApplicationSettingsPage.xaml
/// </summary>
public partial class ApplicationSettingsPage : GenericBasePage<ApplicationSettingsPageViewModel>, INotifyPropertyChanged
{
    public ApplicationSettingsPage(ApplicationSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
