using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TodoApp2
{
    public class SingletonPopup : Popup
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(string), typeof(SingletonPopup), new PropertyMetadata());

        public string SelectedColor
        {
            get => (string)GetValue(SelectedColorProperty);
            set
            {
                SetValue(SelectedColorProperty, value);
                SelectedColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectedColorChanged;
    }
}
