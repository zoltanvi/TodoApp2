using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : GenericBasePage<SettingsPageViewModel>, INotifyPropertyChanged
{
    public SettingsPage(SettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}