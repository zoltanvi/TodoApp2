using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace TodoApp2
{
    internal class PutCaretAtTheEndRTBBehavior : Behavior<UIElement>
    {
        private RichTextBox m_RichTextBox;

        protected override void OnAttached()
        {
            base.OnAttached();


            if (AssociatedObject is RichTextBox richTextBox)
            {
                m_RichTextBox = richTextBox;
                m_RichTextBox.GotFocus += RichTextBox_GotFocus;
            }
        }

        protected override void OnDetaching()
        {
            if (m_RichTextBox != null)
            {
                m_RichTextBox.GotFocus -= RichTextBox_GotFocus;
            }

            base.OnDetaching();
        }

        private void RichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            m_RichTextBox.CaretPosition = m_RichTextBox.Document.ContentEnd;
        }
    }
}
