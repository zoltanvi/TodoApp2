using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TodoApp2
{
    public class SubmitEscapeTextBox : TextBox
    {
        public static readonly DependencyProperty EscapePresssedCommandProperty = DependencyProperty.Register(nameof(EscapePressedCommand), typeof(ICommand), typeof(SubmitEscapeTextBox));
        public static readonly DependencyProperty EnterPressedCommandProperty = DependencyProperty.Register(nameof(EnterPressedCommand), typeof(ICommand), typeof(SubmitEscapeTextBox));

        public ICommand EscapePressedCommand
        {
            get { return (ICommand)GetValue(EscapePresssedCommandProperty); }
            set { SetValue(EscapePresssedCommandProperty, value); }
        }

        public ICommand EnterPressedCommand
        {
            get { return (ICommand)GetValue(EnterPressedCommandProperty); }
            set { SetValue(EnterPressedCommandProperty, value); }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            bool escape = e.Key == Key.Escape;
            bool enter = e.Key == Key.Enter;

            if (escape)
            {
                EscapePressedCommand?.Execute(null);
            }

            if (enter)
            {
                EnterPressedCommand?.Execute(null);
            }
        }
    }
}
