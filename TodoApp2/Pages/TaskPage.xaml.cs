using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            ViewModel.PersistTaskList();
        }

        /// <summary>
        /// Preview the input into the message box and respond as required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewTaskTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Get the text box
            if (sender is TextBox textBox)
            {
                // Check if we have pressed enter
                if (e.Key == Key.Enter)
                {
                    // If we have SHIFT pressed
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        // Add a new line at the point where the cursor is
                        var index = textBox.CaretIndex;

                        // Insert the new line
                        textBox.Text = textBox.Text.Insert(index, Environment.NewLine);

                        // Shift the caret forward to the newline
                        textBox.CaretIndex = index + Environment.NewLine.Length;

                        // Mark this key as handled by us
                        e.Handled = true;
                    }
                    else
                    {
                        // Add the task
                        ViewModel.AddTask();
                    }

                    // Mark the key as handled
                    e.Handled = true;
                }
            }
        }
    }
}
