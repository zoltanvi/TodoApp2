using Modules.Common.Navigation;
using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskReminderPage.xaml
    /// </summary>
    public partial class TaskReminderPage : GenericBasePage<TaskReminderPageViewModel>, ITaskReminderPage
    {
        public TaskReminderPage(TaskReminderPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}