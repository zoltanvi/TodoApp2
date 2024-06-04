using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for ThemeSettingsPage.xaml
/// </summary>
public partial class ThemeSettingsPage : GenericBasePage<ThemeSettingsPageViewModel>, INotifyPropertyChanged
{
    public ThemeSettingsPage(ThemeSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
