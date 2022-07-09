using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for NotePage.xaml
    /// </summary>
    public partial class NotePage : BasePage<NotePageViewModel>
    {
        public NotePage()
        {
            InitializeComponent();
        }

        public NotePage(NotePageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollBar scrollbar = sender as ScrollBar;
            HorizontalScrollViewer.ScrollToHorizontalOffset(scrollbar.Value);
        }

        private void HorizontalScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            HorizontalScrollBarControl.Value += e.HorizontalChange;
        }

        private void NoteContentTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    ViewModel.SaveNoteContent();
                }
                if (e.Key == Key.Tab)
                {
                    string text = textBox.Text;
                    string selectedText = textBox.SelectedText;
                    int selectionLength = textBox.SelectionLength;
                    int caretIndex = textBox.CaretIndex;

                    if (ContainsLineEndChar(selectedText))
                    {
                        if (selectionLength > 0)
                        {
                            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                            {
                                int correctedStartIndex = SearchStartOfLine(text, caretIndex, out int offset);
                                if (correctedStartIndex == -1)
                                {
                                    correctedStartIndex = 0;
                                }

                                if (SearchForward(text, selectedText, caretIndex, selectionLength))
                                {
                                    textBox.Text = ShiftRight(text, correctedStartIndex, selectionLength + offset - 1);
                                }
                            }
                            else
                            {

                            }

                        }
                    }
                    else
                    {
                        if (selectionLength > 0)
                        {
                            text = text.Remove(caretIndex, selectionLength);
                        }

                        textBox.Text = text.Insert(caretIndex, "    ");
                        textBox.CaretIndex = caretIndex + 4;
                    }

                    e.Handled = true;
                }
            }
        }

        private bool ContainsLineEndChar(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\n')
                {
                    return true;
                }
            }
            return false;
        }

        private bool SearchForward(string input, string subString, int startPosition, int length)
        {
            if (input.Length < startPosition + length)
            {
                return false;
            }

            for (int i = startPosition, j = 0; i < startPosition + length; i++, j++)
            {
                if (input[i] != subString[j])
                {
                    return false;
                }
            }

            return true;
        }

        private int SearchStartOfLine(string input, int selectionStart, out int offset)
        {
            offset = 0;
            for (int i = selectionStart; i >= 0; i--)
            {
                if (input[i] == '\n')
                {
                    return i;
                }
                offset++;
            }

            return -1;
        }

        private string ShiftRight(string input, int startPosition, int length)
        {
            StringBuilder sb = new StringBuilder();

            if (startPosition == 0)
            {
                sb.Append(' ', 4);
            }
            else
            {
                sb.Append(input, 0, startPosition);
            }

            for (int i = startPosition; i < startPosition + length; i++)
            {
                sb.Append(input[i]);

                if (input[i] == '\n')
                {
                    sb.Append(' ', 4);
                }
            }

            sb.Append(input, startPosition + length, input.Length - (startPosition + length));

            return sb.ToString();
        }

    }
}
