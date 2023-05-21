using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TextEditorQuickActionsSettingsPage.xaml
    /// </summary>
    public partial class TextEditorQuickActionsSettingsPage : BasePage<TextEditorQuickActionsSettingsPageViewModel>, INotifyPropertyChanged
    {
        public TextEditorQuickActionsSettingsPage(TextEditorQuickActionsSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
