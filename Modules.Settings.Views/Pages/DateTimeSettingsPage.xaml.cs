using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for DateTimeSettingsPage.xaml
/// </summary>
public partial class DateTimeSettingsPage : GenericBasePage<DateTimeSettingsPageViewModel>, INotifyPropertyChanged
{
    public DateTimeSettingsPage(DateTimeSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
