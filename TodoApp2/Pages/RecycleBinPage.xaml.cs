using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class RecycleBinPage : GenericBasePage<RecycleBinPageViewModel>
    {
        public RecycleBinPage(RecycleBinPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}