using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for ReminderPage.xaml
    /// </summary>
    public partial class ReminderEditorPage : GenericBasePage<ReminderEditorPageViewModel>
    {
        public ReminderEditorPage(ReminderEditorPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}