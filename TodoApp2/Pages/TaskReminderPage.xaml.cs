using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskReminderPage.xaml
    /// </summary>
    public partial class TaskReminderPage : BasePage<TaskReminderPageViewModel>
    {
        public TaskReminderPage()
        {
            InitializeComponent();
        }

        public TaskReminderPage(TaskReminderPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}