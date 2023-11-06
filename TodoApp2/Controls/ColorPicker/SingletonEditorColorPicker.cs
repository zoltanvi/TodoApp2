using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    public class SingletonEditorColorPicker : Button
    {
        public static readonly DependencyProperty SelectedColorStringProperty = DependencyProperty.Register(nameof(SelectedColorString), typeof(string), typeof(SingletonEditorColorPicker), new PropertyMetadata(CoreConstants.ColorName.Transparent));
        public static readonly DependencyProperty AppliedColorStringProperty = DependencyProperty.Register(nameof(AppliedColorString), typeof(string), typeof(SingletonEditorColorPicker), new PropertyMetadata(CoreConstants.ColorName.Transparent));

        private SingletonPopup _popup;

        public ICommand OpenPopupCommand { get; }

        public string SelectedColorString
        {
            get => (string)GetValue(SelectedColorStringProperty);
            set => SetValue(SelectedColorStringProperty, value);
        }

        public string AppliedColorString
        {
            get => (string)GetValue(AppliedColorStringProperty);
            set => SetValue(AppliedColorStringProperty, value);
        }

        public SingletonEditorColorPicker()
        {
            OpenPopupCommand = new RelayCommand(OpenPopup);
        }

        private void OpenPopup()
        {
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

        // Called when the left button has been clicked
        protected override void OnClick()
        {
            base.OnClick();

            ApplyDisplayColor();
        }

        private void ApplyDisplayColor()
        {
            if (AppliedColorString == SelectedColorString)
            {
                AppliedColorString = string.Empty;
            }

            AppliedColorString = SelectedColorString;
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

            ApplyDisplayColor();
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            IsEnabled = true;
            _popup.Closed -= OnPopupClosed;
        }
    }
}
