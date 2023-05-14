using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for NotePageSettingsPage.xaml
    /// </summary>
    public partial class NotePageSettingsPage : BasePage<NotePageSettingsPageViewModel>, INotifyPropertyChanged
    {
        public NotePageSettingsPage(NotePageSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
