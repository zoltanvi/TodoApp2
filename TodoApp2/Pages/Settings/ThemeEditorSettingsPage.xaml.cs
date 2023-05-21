using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for ThemeEditorSettingsPage.xaml
    /// </summary>
    public partial class ThemeEditorSettingsPage : BasePage<ThemeEditorSettingsPageViewModel>, INotifyPropertyChanged
    {
        public ThemeEditorSettingsPage(ThemeEditorSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
