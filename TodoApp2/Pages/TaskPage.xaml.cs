using System;
using System.ComponentModel;
using System.Text;
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
        private const char FormatCharacter = '`';


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

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && 
                    e.Key == Key.B)
                {
                    int caretIndex = textBox.CaretIndex;

                    string text = textBox.Text;
                    string selectedText = textBox.SelectedText;
                    int selectionStart = textBox.SelectionStart;
                    int selectionLength = textBox.SelectionLength;

                    textBox.Text = EncloseInFormatCharacter(text, selectedText, selectionStart, selectionLength);

                    // Set the new cursor position to right after the modified part
                    textBox.CaretIndex = caretIndex + selectionLength + 2;

                    // Mark this key as handled by us
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Encloses the selected part of a string in format characters.
        /// </summary>
        /// <param name="text">The whole text to modify.</param>
        /// <param name="selectedText">The selected part of the text.</param>
        /// <param name="selectionStart">The index where the selection starts.</param>
        /// <param name="selectionLength">The selection length.</param>
        /// <returns>Returns the modified text where the selected part is enclosed in format characters.</returns>
        private string EncloseInFormatCharacter(string text, string selectedText, int selectionStart, int selectionLength)
        {
            // From the start until before the selection
            string before = text.Substring(0, selectionStart);
            
            // From after the selection until the end
            string after = text.Substring(selectionStart + selectionLength, text.Length - selectionStart - selectionLength);

            return $"{before}{FormatCharacter}{selectedText}{FormatCharacter}{after}";
        }
    }
}
