using System.ComponentModel;
using System.Net.Mime;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2.Pages
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : BasePage<TaskListViewModel>
    {
        public TaskPage()
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Closing += MainWindowOnClosing;
            }
        }

        /// <summary>
        /// When the application get closed somehow (intentionally or unintentionally)
        /// try to save the task items into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (ViewModel is TaskListViewModel viewModel)
            {
                viewModel.PersistTaskList();
            }
        }
    }
}
