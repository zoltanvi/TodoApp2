using System;
using System.Diagnostics.CodeAnalysis;
using TodoApp2.Core;

namespace TodoApp2
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class UIScaler : BaseViewModel
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
            private const double OriginalHuge = 28;
            private const double OriginalLargeIcon = 30;
            private const double OriginalGiant = 40;

            public double Smallest => OriginalSmallest * StaticScaleValue;
            public double Smaller => OriginalSmaller * StaticScaleValue;
            public double Small => OriginalSmall * StaticScaleValue;
            public double Medium => OriginalMedium * StaticScaleValue;
            public double Regular => OriginalRegular * StaticScaleValue;
            public double Large => OriginalLarge * StaticScaleValue;
            public double Huge => OriginalHuge * StaticScaleValue;
            public double Giant => OriginalGiant * StaticScaleValue;
            public double RegularIcon => OriginalRegularIcon * StaticScaleValue;
            public double LargeIcon => OriginalLargeIcon * StaticScaleValue;
        }
        
        private const double s_OriginalScalingPercent = 100;
        private const double OriginalSideMenuWidth = 300;
        private const double OriginalTextBoxMaxHeight = 162;
        private const double OriginalColorPickerHeight = 22;
        private const double OriginalColorPickerWidth = 32;
        private const double OriginalColorPickerItemSize = 16;
        private const double OriginalSettingsComboboxWidth = 110;
        private double m_ScalingPercent = s_OriginalScalingPercent;
    
        public static double StaticScaleValue { get; set; } = 1;
        public double ScaleValue => StaticScaleValue;

        public FontSizes FontSize { get; } = new FontSizes();
        public double SideMenuWidth => OriginalSideMenuWidth * ScaleValue;
        public double TextBoxMaxHeight => OriginalTextBoxMaxHeight * ScaleValue;
        public double ColorPickerHeight => OriginalColorPickerHeight * ScaleValue;
        public double ColorPickerWidth => OriginalColorPickerWidth * ScaleValue;
        public double ColorPickerItemSize => OriginalColorPickerItemSize * ScaleValue;
        public double SettingsComboboxWidth => OriginalSettingsComboboxWidth * ScaleValue;
        public double ColorPickerDropDownHeight => 44 + 16 + (5 * ColorPickerItemSize);

        public event EventHandler Zoomed;

        public void ZoomOut()
        {
            Zoom(false);
        }

        public void ZoomIn()
        {
            Zoom(true);
        }

        private void Zoom(bool zoomIn)
        {
            double zoomOffset = 0;
            const double maxScalingPercent = 400;
            const double minScalingPercent = 50;

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
            StaticScaleValue = m_ScalingPercent / s_OriginalScalingPercent;

            OnPropertyChanged(string.Empty);

            Zoomed?.Invoke(this, EventArgs.Empty);
            IoC.MessageService.ShowInfo($"{m_ScalingPercent} %");
        }
    }
}
