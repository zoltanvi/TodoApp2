using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace TodoApp2
{
    public static class InputBindingsManager
    {
        public static readonly DependencyProperty UpdatePropertyOnEnterPressProperty = DependencyProperty.RegisterAttached(
                "UpdatePropertyOnEnterPress", typeof(DependencyProperty), typeof(InputBindingsManager), new PropertyMetadata(null, OnUpdatePropertyOnEnterPressPropertyChanged));

        public static readonly DependencyProperty FormatTextOnTargetUpdatedProperty = DependencyProperty.RegisterAttached(
               "FormatTextOnTargetUpdated", typeof(DependencyProperty), typeof(InputBindingsManager), new PropertyMetadata(null, OnFormatTextOnTargetUpdatedPropertyChanged));

        private static readonly FontFamily ConsolasFont = new FontFamily("Consolas");
        private static readonly SolidColorBrush HighLightColor = new SolidColorBrush(Color.FromRgb(57, 154, 194));

        static InputBindingsManager()
        {
        }

        #region UpdatePropertyOnEnterPress

        public static DependencyProperty GetUpdatePropertyOnEnterPress(DependencyObject dependencyObject)
        {
            return (DependencyProperty)dependencyObject.GetValue(UpdatePropertyOnEnterPressProperty);
        }

        public static void SetUpdatePropertyOnEnterPress(DependencyObject dependencyObject, DependencyProperty value)
        {
            dependencyObject.SetValue(UpdatePropertyOnEnterPressProperty, value);
        }

        private static void OnUpdatePropertyOnEnterPressPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is UIElement element)
            {
                if (e.OldValue != null)
                {
                    element.PreviewKeyDown -= HandlePreviewKeyDown;
                }

                if (e.NewValue != null)
                {
                    element.PreviewKeyDown += new KeyEventHandler(HandlePreviewKeyDown);
                }
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoUpdateSource(e.Source);
            }
        }

        private static void DoUpdateSource(object source)
        {
            DependencyProperty property = GetUpdatePropertyOnEnterPress(source as DependencyObject);

            if (property != null && source is UIElement uiElement)
            {
                BindingExpression binding = BindingOperations.GetBindingExpression(uiElement, property);
                binding?.UpdateSource();
            }
        }

        #endregion UpdatePropertyOnEnterPress

        #region FormatTextOnTargetUpdated

        public static DependencyProperty GetFormatTextOnTargetUpdated(DependencyObject dependencyObject)
        {
            return (DependencyProperty)dependencyObject.GetValue(FormatTextOnTargetUpdatedProperty);
        }

        public static void SetFormatTextOnTargetUpdated(DependencyObject dependencyObject, DependencyProperty value)
        {
            dependencyObject.SetValue(FormatTextOnTargetUpdatedProperty, value);
        }

        private static void OnFormatTextOnTargetUpdatedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is FrameworkElement element)
            {
                if (e.OldValue != null)
                {
                    element.TargetUpdated -= OnTargetUpdated;
                }

                if (e.NewValue != null)
                {
                    element.TargetUpdated += OnTargetUpdated;
                }
            }
        }

        private static void OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                TextFormatter textFormatter = new TextFormatter();
                textFormatter.FormatText(textBlock);
            }
        }

        #endregion FormatTextOnTargetUpdated
    }
}