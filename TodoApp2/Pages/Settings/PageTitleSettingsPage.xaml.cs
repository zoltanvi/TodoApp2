using System.ComponentModel;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for PageTitleSettingsPage.xaml
    /// </summary>
    public partial class PageTitleSettingsPage : BasePage<PageTitleSettingsPageViewModel>, INotifyPropertyChanged
    {
        public PageTitleSettingsPage(PageTitleSettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
