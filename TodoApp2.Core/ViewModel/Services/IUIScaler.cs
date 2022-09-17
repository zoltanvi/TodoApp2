using System;

namespace TodoApp2
{
    public interface IUIScaler
    {
        double ColorPickerDropDownWidth { get; }
        double ColorPickerHeight { get; }
        double ColorPickerItemSize { get; }
        double ColorPickerOnlyDropDownWidth { get; }
        double ColorPickerWidth { get; }
        UIScaler.FontSizes FontSize { get; }
        double ScaleValue { get; }
        double SettingsComboboxWidth { get; }
        double SideMenuWidth { get; }
        double TextBoxMaxHeight { get; }

        event EventHandler Zoomed;

        void SetScaling(double value);
        void ZoomIn();
        void ZoomOut();
    }
}