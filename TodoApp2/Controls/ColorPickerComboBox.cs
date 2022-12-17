using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    public class ColorPickerComboBox : ComboBox
    {
        public static readonly DependencyProperty DisplayColorProperty = DependencyProperty.Register(nameof(DisplayColor), typeof(string), typeof(ColorPickerComboBox), new PropertyMetadata());
        public static readonly DependencyProperty AppliedColorProperty = DependencyProperty.Register(nameof(AppliedColor), typeof(string), typeof(ColorPickerComboBox), new PropertyMetadata());

        public string DisplayColor
        {
            get => (string)GetValue(DisplayColorProperty);
            set => SetValue(DisplayColorProperty, value);
        }

        public string AppliedColor
        {
            get => (string)GetValue(AppliedColorProperty);
            set => SetValue(AppliedColorProperty, value);
        }

        public ICommand ApplyDisplayColorCommand { get; set; }

        public ColorPickerComboBox()
        {
            ApplyDisplayColorCommand = new RelayCommand(ApplyDisplayColor);
            SelectionChanged += OnSelectionChanged;
        }

        private void ApplyDisplayColor()
        {
            if (AppliedColor == DisplayColor)
            {
                AppliedColor = string.Empty;
            }

            AppliedColor = DisplayColor;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                DisplayColor = (string)e.AddedItems[0];

                ApplyDisplayColor();
            }
        }
    }
}
