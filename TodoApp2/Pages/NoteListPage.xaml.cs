using TodoApp2.Core;

namespace TodoApp2
{
    public partial class NoteListPage : BasePage<NoteListPageViewModel>
    {
        public NoteListPage(NoteListPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}