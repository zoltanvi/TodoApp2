using Modules.Common.Views.Pages;
using System.ComponentModel;

namespace Modules.Settings.Views.Pages;

/// <summary>
/// Interaction logic for NotePageSettingsPage.xaml
/// </summary>
public partial class NotePageSettingsPage : GenericBasePage<NotePageSettingsPageViewModel>, INotifyPropertyChanged
{
    public NotePageSettingsPage(NotePageSettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
