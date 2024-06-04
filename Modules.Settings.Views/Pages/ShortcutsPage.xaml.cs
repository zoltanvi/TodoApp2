using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class ShortcutsPage : GenericBasePage<ShortcutsPageViewModel>, INotifyPropertyChanged
{
    public ShortcutsPage(ShortcutsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}