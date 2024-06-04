using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for PageTitleSettingsPage.xaml
/// </summary>
public partial class PageTitleSettingsPage : GenericBasePage<PageTitleSettingsPageViewModel>, INotifyPropertyChanged
{
    public PageTitleSettingsPage(PageTitleSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
