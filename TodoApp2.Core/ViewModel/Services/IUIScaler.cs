using System;

namespace TodoApp2.Core
{
    public interface IUIScaler
    {
        double ColorPickerDropDownWidth { get; }
        double ColorPickerHeight { get; }
        double ColorPickerItemSize { get; }
        double ColorPickerWidth { get; }
        UIScaler.FontSizes FontSize { get; }
        double ScaleValue { get; }
        double SideMenuWidth { get; }
        double TextBoxMaxHeight { get; }
        double TextEditorToggleWidth { get; }
        double TaskCheckBoxWidth { get; }
        double SliderHeight { get; }
        double SliderThumbHeight { get; }
        double SliderThumbWidth { get; }

        event EventHandler<ZoomedEventArgs> Zoomed;

        void SetScaling(double value);
        void ZoomIn();
        void ZoomOut();
    }
}