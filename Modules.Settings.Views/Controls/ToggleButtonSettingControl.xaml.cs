using System.Windows;
using System.Windows.Controls;

namespace Modules.Settings.Views.Controls
{
    /// <summary>
    /// Interaction logic for ToggleButtonSettingControl.xaml
    /// </summary>
    public partial class ToggleButtonSettingControl : UserControl
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(object), typeof(ToggleButtonSettingControl));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(ToggleButtonSettingControl));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked", typeof(bool), typeof(ToggleButtonSettingControl));

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public ToggleButtonSettingControl()
        {
            InitializeComponent();
        }
    }
}
