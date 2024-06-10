using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.Notes.Views.Pages;

public partial class NoteListPage : GenericBasePage<NoteListPageViewModel>, INoteListPage
{
    public NoteListPage(NoteListPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}