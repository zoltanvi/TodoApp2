using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : BasePage<TaskPageViewModel>
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
            ViewModel.PersistTaskList();
        }

        /// <summary>
        /// Preview the input into the message box and respond as required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewTaskTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Add task on enter, handle modifier keys
            TextBoxPreviewKeyDownHelper.TextBox_PreviewKeyDown(sender, e, ViewModel.AddTask);
        }

    }
}