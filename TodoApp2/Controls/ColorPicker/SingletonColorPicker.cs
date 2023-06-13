using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TodoApp2
{
    public class SingletonColorPicker : ToggleButton
    {
        public static readonly DependencyProperty SelectedColorStringProperty = DependencyProperty.Register(nameof(SelectedColorString), typeof(string), typeof(SingletonColorPicker), new PropertyMetadata());

        private SingletonPopup _popup;

        public string SelectedColorString
        {
            get => (string)GetValue(SelectedColorStringProperty);
            set => SetValue(SelectedColorStringProperty, value);
        }

        protected override void OnToggle()
        {
            base.OnToggle();

            if (IsChecked.HasValue && IsChecked.Value)
            {
                if (_popup == null)
                {
                    _popup = (SingletonPopup)FindResource("ColorPickerPopup");
                }

                _popup.Closed += OnPopupClosed;
                _popup.LostFocus += OnPopupLostFocus;
                _popup.SelectedColor = SelectedColorString;
                _popup.SelectedColorChanged += OnPopupSelectedColorChanged;
                _popup.PlacementTarget = this;
                _popup.IsOpen = true;
                //IsEnabled = false;
                //IsHitTestVisible = false;

            }
            else
            {
                _popup.IsOpen = false;
            }
        }

        private void OnPopupLostFocus(object sender, RoutedEventArgs e)
        {
            OnPopupClosed(sender, EventArgs.Empty);
            _popup.LostFocus -= OnPopupLostFocus;
        }

        private void OnPopupSelectedColorChanged(object sender, EventArgs e)
        {
            SelectedColorString = _popup.SelectedColor;
            _popup.IsOpen = false;
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            IsChecked = false;
            IsEnabled = true;
            _popup.SelectedColorChanged -= OnPopupSelectedColorChanged;
            _popup.Closed -= OnPopupClosed;
        }

    }
}
