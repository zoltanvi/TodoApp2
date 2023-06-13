using System;
using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    public class SingletonColorPicker : Button
    {
        public static readonly DependencyProperty SelectedColorStringProperty = DependencyProperty.Register(nameof(SelectedColorString), typeof(string), typeof(SingletonColorPicker), new PropertyMetadata());

        private SingletonPopup _popup;

        public string SelectedColorString
        {
            get => (string)GetValue(SelectedColorStringProperty);
            set => SetValue(SelectedColorStringProperty, value);
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (_popup == null)
            {
                _popup = PopupService.Popup;
            }

            _popup.IsOpen = false;

            // First register to clear previous registration if there is any
            _popup.RegisterSelectedColorChangeEvent(OnPopupSelectedColorChanged);
            _popup.SelectedColor = SelectedColorString;

            _popup.Closed += OnPopupClosed;
            _popup.LostFocus += OnPopupLostFocus;

            _popup.PlacementTarget = this;
            _popup.IsOpen = true;
        }

        private void OnPopupLostFocus(object sender, RoutedEventArgs e)
        {
            OnPopupClosed(sender, EventArgs.Empty);
            _popup.LostFocus -= OnPopupLostFocus;
        }

        private void OnPopupSelectedColorChanged()
        {
            SelectedColorString = _popup.SelectedColor;
            _popup.IsOpen = false;
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            IsEnabled = true;
            _popup.Closed -= OnPopupClosed;
        }

    }
}
