using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TodoApp2
{
    internal class CustomScrollViewer : ScrollViewer
    {
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (!e.Handled)
            {
                e.Handled = true;
                RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = this
                });
            }
        }

        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
        }
    }
}
