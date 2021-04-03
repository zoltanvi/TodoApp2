using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for ReminderPage.xaml
    /// </summary>
    public partial class ReminderEditorPage : BasePage<ReminderEditorPageViewModel>
    {
        public ReminderEditorPage()
        {
            InitializeComponent();
        }

        public ReminderEditorPage(ReminderEditorPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}