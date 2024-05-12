using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class RecycleBinPage : BasePage<RecycleBinPageViewModel>
    {
        public RecycleBinPage(RecycleBinPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}