using System.Windows;
using System.Windows.Controls;

namespace Modules.Settings.Views.Controls;

/// <summary>
/// Interaction logic for ShortcutItemControl.xaml
/// </summary>
public partial class ShortcutItemControl : UserControl
{
    public static readonly DependencyProperty Key1Property = DependencyProperty.Register(nameof(Key1), typeof(string), typeof(ShortcutItemControl));
    public static readonly DependencyProperty Key2Property = DependencyProperty.Register(nameof(Key2), typeof(string), typeof(ShortcutItemControl));
    public static readonly DependencyProperty Key3Property = DependencyProperty.Register(nameof(Key3), typeof(string), typeof(ShortcutItemControl));
    public static readonly DependencyProperty Key4Property = DependencyProperty.Register(nameof(Key4), typeof(string), typeof(ShortcutItemControl));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShortcutItemControl));

    public string Key1
    {
        get => (string)GetValue(Key1Property);
        set => SetValue(Key1Property, value);
    }

    public string Key2
    {
        get => (string)GetValue(Key2Property);
        set => SetValue(Key2Property, value);
    }

    public string Key3
    {
        get => (string)GetValue(Key3Property);
        set => SetValue(Key3Property, value);
    }

    public string Key4
    {
        get => (string)GetValue(Key4Property);
        set => SetValue(Key4Property, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public ShortcutItemControl()
    {
        InitializeComponent();
    }
}
