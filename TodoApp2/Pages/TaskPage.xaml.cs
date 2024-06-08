using Modules.Common.Navigation;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Views.Pages;
using System.ComponentModel;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : GenericBasePage<TaskPageViewModel>, ITaskPage
    {
        private readonly TaskListService _taskListService;

        public TaskPage(TaskPageViewModel viewModel, TaskListService taskListService) : base(viewModel)
        {
            _taskListService = taskListService;

            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Closing += MainWindowOnClosing;
            }

            ViewModel.ScrollIntoViewAction = MyTaskListControl.ScrollToItem;
            IoC.OneEditorOpenService.OnEditModeEnd = FocusBottomTextEditor;
            Mediator.Register(OnFocusRequested, ViewModelMessages.FocusBottomTextEditor);
        }

        private void OnFocusRequested(object obj)
        {
            BottomTextEditor.SetFocus();
        }

        private void FocusBottomTextEditor()
        {
            BottomTextEditor.SetFocus();
        }

        /// <summary>
        /// When the application get closed somehow (intentionally or unintentionally)
        /// try to save the task items into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            _taskListService.PersistTaskList();
        }

    }
}