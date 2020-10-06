using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System;

namespace TodoApp2
{
    public class TextBoxPreviewKeyDownHelper
    {
        public const char FormatCharacter = '`';

        /// <summary>
        /// Preview the input into the message box and respond as required.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="enterAction">The action to execute when the enter key is pressed.</param>
        public static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e, Action enterAction)
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
                        enterAction();
                    }

                    // Mark the key as handled
                    e.Handled = true;
                }

                // Enclose selected text in format characters if only [Ctrl + B] is pressed
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
                    !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
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
        private static string EncloseInFormatCharacter(string text, string selectedText, int selectionStart, int selectionLength)
        {
            // From the start until before the selection
            string before = text.Substring(0, selectionStart);

            // From after the selection until the end
            string after = text.Substring(selectionStart + selectionLength, text.Length - selectionStart - selectionLength);

            return $"{before}{FormatCharacter}{selectedText}{FormatCharacter}{after}";
        }
    }
}
