using Modules.Common.DataBinding;
using Modules.Common.Views.Services;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Common.Views.Controls;

public class SingletonColorPicker : Button
{
    public static readonly DependencyProperty SelectedColorStringProperty = 
        DependencyProperty.Register(nameof(SelectedColorString), typeof(string), typeof(SingletonColorPicker), new PropertyMetadata());

    public static readonly DependencyProperty ColorChangedNotificationProperty = 
        DependencyProperty.Register(nameof(ColorChangedNotification), typeof(INotifiableObject), typeof(SingletonColorPicker));

    private SingletonPopup _popup;

    public string SelectedColorString
    {
        get => (string)GetValue(SelectedColorStringProperty);
        set => SetValue(SelectedColorStringProperty, value);
    }

    public INotifiableObject ColorChangedNotification
    {
        get { return (INotifiableObject)GetValue(ColorChangedNotificationProperty); }
        set { SetValue(ColorChangedNotificationProperty, value); }
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
        ColorChangedNotification?.Notify();
    }
}
