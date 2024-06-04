using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for TextEditorQuickActionsSettingsPage.xaml
/// </summary>
public partial class TextEditorQuickActionsSettingsPage : GenericBasePage<TextEditorQuickActionsSettingsPageViewModel>, INotifyPropertyChanged
{
    public TextEditorQuickActionsSettingsPage(TextEditorQuickActionsSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
