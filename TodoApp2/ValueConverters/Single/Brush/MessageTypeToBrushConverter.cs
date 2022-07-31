using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class MessageTypeToBrushConverter : BaseValueConverter
    {
        public string InfoBrushName { get; set; }
        public string WarningBrushName { get; set; }
        public string ErrorBrushName { get; set; }
        public string SuccessBrushName { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is MessageType messageType)
            {
                switch (messageType)
                {
                    case MessageType.Warning:
                        return (SolidColorBrush)Application.Current.TryFindResource(WarningBrushName);
                    case MessageType.Info:
                        return (SolidColorBrush)Application.Current.TryFindResource(InfoBrushName);
                    case MessageType.Error:
                        return (SolidColorBrush)Application.Current.TryFindResource(ErrorBrushName);
                    case MessageType.Success:
                        return (SolidColorBrush)Application.Current.TryFindResource(InfoBrushName);
                }
            }

            return null;
        }
    }
}
