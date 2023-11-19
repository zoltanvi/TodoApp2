using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using TodoApp2.Core;

namespace TodoApp2
{
    public class SingletonPopup : Popup
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(string), typeof(SingletonPopup), new PropertyMetadata());

        private Action _selectedColorChangedAction;

        public string SelectedColor
        {
            get => (string)GetValue(SelectedColorProperty);
            set
            {
                DebugLogger.Log($"Popup: {SelectedColor} -> {value}");
                SetValue(SelectedColorProperty, value);
                _selectedColorChangedAction?.Invoke();
            }
        }

        public void RegisterSelectedColorChangeEvent(Action action)
        {
            // Allows only 1 subscriber at all times
            _selectedColorChangedAction = action;
        }
    }
}
