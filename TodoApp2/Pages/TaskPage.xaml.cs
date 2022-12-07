﻿using System.ComponentModel;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : BasePage<TaskPageViewModel>
    {
        private readonly TaskListService m_TaskListService;

        public TaskPage(TaskPageViewModel viewModel, TaskListService taskListService) : base(viewModel)
        {
            m_TaskListService = taskListService;

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
            m_TaskListService.PersistTaskList();
        }
    }
}