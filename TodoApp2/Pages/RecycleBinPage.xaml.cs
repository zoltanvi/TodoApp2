using Modules.Common.Navigation;
using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class RecycleBinPage : GenericBasePage<RecycleBinPageViewModel>, IRecycleBinPage
    {
        public RecycleBinPage(RecycleBinPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}