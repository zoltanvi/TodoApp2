using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace TodoApp2
{
    internal class TimePickerTextBox : ClickSelectTextBox
    {
        private static readonly Regex m_Regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        public TimePickerTextBox()
        {
            DataObject.AddPastingHandler(this, TextBoxPasting);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!IsTextAllowed(e.Text))
            {
                e.Handled = true;
            }

            base.OnPreviewTextInput(e);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !m_Regex.IsMatch(text);
        }
    }
}
