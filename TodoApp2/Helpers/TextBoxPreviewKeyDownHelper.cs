using System;
using System.Windows.Input;

namespace TodoApp2
{
    public class TextBoxPreviewKeyDownHelper
    {
        /// <summary>
        /// Preview the input into the message box and respond as required.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="enterAction">The action to execute when the enter key is pressed.</param>
        public static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e, Action enterAction)
        {
            if (sender is TextEditorBox richTextBox)
            {
                // Check if we have pressed enter
                if (e.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    // Since the focus not necessary changes, it needs a manual update
                    richTextBox.UpdateContent();

                    //Keyboard.ClearFocus();

                    // TODO: Debug why it is not working
                    // Clear undo stack
                    richTextBox.IsUndoEnabled = false;
                    richTextBox.IsUndoEnabled = true;

                    enterAction();

                    // Mark the key as handled
                    e.Handled = true;
                }
            }
        }
    }
}