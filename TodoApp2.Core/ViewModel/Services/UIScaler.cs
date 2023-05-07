using System;
using TodoApp2.Core;

namespace TodoApp2.Core
{
    public class UIScaler : BaseViewModel, IUIScaler
    {
        public class FontSizes : BaseViewModel
        {
            private const double OriginalSmallest = 10;
            private const double OriginalSmaller = 12;
            private const double OriginalSmall = 14;
            private const double OriginalMedium = 16;
            private const double OriginalRegular = 18;
            private const double OriginalRegularIcon = 20;
            private const double OriginalLarge = 22;
            private const double OriginalLarger = 24;
            private const double OriginalHuge = 28;
            private const double OriginalLargeIcon = 30;
            private const double OriginalGiant = 40;

            public double Smallest => OriginalSmallest * StaticScaleValue;
            public double Smaller => OriginalSmaller * StaticScaleValue;
            public double Small => OriginalSmall * StaticScaleValue;
            public double Medium => OriginalMedium * StaticScaleValue;
            public double Regular => OriginalRegular * StaticScaleValue;
            public double Large => OriginalLarge * StaticScaleValue;
            public double Larger => OriginalLarger * StaticScaleValue;
            public double Huge => OriginalHuge * StaticScaleValue;
            public double Giant => OriginalGiant * StaticScaleValue;
            public double RegularIcon => OriginalRegularIcon * StaticScaleValue;
            public double LargeIcon => OriginalLargeIcon * StaticScaleValue;
        }

        private const double OriginalScalingPercent = 100;
        private const double OriginalSideMenuWidth = 220;
        private const double OriginalSideMenuMinimumWidth = 180;
        private const double OriginalTextBoxMaxHeight = 400;
        private const double OriginalColorPickerHeight = 31;
        private const double OriginalColorPickerWidth = 56;
        private const double OriginalColorPickerItemSize = 21;
        private const double OriginalTextEditorToggleWidth = 15;
        private const double OriginalThemeItemWidth = 140;
        private const double OriginalThemeItemHeight = 108;

        private const int ColorPickerColumns = 10;

        private double m_ScalingPercent = OriginalScalingPercent;

        public static double StaticScaleValue { get; private set; } = 1;
        public double ScaleValue => StaticScaleValue;

        public FontSizes FontSize { get; } = new FontSizes();
        public double SideMenuWidth => OriginalSideMenuWidth * ScaleValue;
        public double SideMenuMinimumWidth => OriginalSideMenuMinimumWidth * ScaleValue; // Not decided yet
        public double TextBoxMaxHeight => OriginalTextBoxMaxHeight * ScaleValue;
        public double ColorPickerHeight => OriginalColorPickerHeight * ScaleValue;
        public double ColorPickerWidth => OriginalColorPickerWidth * ScaleValue;
        public double ColorPickerHalfWidth => (OriginalColorPickerWidth * ScaleValue) / 2;
        public double ColorPickerItemSize => OriginalColorPickerItemSize * ScaleValue;
        public double TextEditorToggleWidth => OriginalTextEditorToggleWidth * ScaleValue;
        public double ColorPickerDropDownWidth => 44 + 16 + (ColorPickerColumns * ColorPickerItemSize);
        public double TaskCheckBoxWidth => 8 * ScaleValue;
        public double SliderHeight => 18 * ScaleValue;
        public double SliderThumbHeight => 18 * ScaleValue;
        public double SliderThumbWidth => 15 * ScaleValue;
        public double ScrollbarWidth => 16 * ScaleValue;
        public double NotePageBoxWidth => 17 * ScaleValue;

        public double ThemeItemWidth => OriginalThemeItemWidth * ScaleValue;
        public double ThemeItemHeight => OriginalThemeItemHeight * ScaleValue;

        public static UIScaler Instance { get; } = new UIScaler();

        public event EventHandler<ZoomedEventArgs> Zoomed;

        private UIScaler() { }

        public void ZoomOut()
        {
            Zoom(false);
        }

        public void ZoomIn()
        {
            Zoom(true);
        }

        public void SetScaling(double value)
        {
            value = Math.Round(value, 3);
            bool zoomed = value != StaticScaleValue;
            var oldScaleValue = StaticScaleValue;
            StaticScaleValue = value;
            m_ScalingPercent = StaticScaleValue * OriginalScalingPercent;
            OnPropertyChanged(string.Empty);

            if (zoomed)
            {
                Zoomed?.Invoke(this, new ZoomedEventArgs(oldScaleValue, StaticScaleValue));
            }
        }

        private void Zoom(bool zoomIn)
        {
            double zoomOffset = 0;
            const double maxScalingPercent = 500;
            const double minScalingPercent = 30;

            if (zoomIn && m_ScalingPercent < maxScalingPercent)
            {
                // Zoom in
                zoomOffset = 10;
            }
            else if (!zoomIn && m_ScalingPercent > minScalingPercent)
            {
                // Zoom out
                zoomOffset = -10;
            }

            m_ScalingPercent += zoomOffset;
            SetScaling(m_ScalingPercent / OriginalScalingPercent);
            IoC.MessageService.ShowInfo($"{m_ScalingPercent} %");
        }
    }
}
