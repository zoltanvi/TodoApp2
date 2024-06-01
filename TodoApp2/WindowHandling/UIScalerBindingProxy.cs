using System.Windows;
using TodoApp2.Core;

namespace TodoApp2.WindowHandling;

public class UIScalerBindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new UIScalerBindingProxy();
    }

    public UIScaler UIScaler
    {
        get { return (UIScaler)GetValue(UIScalerProperty); }
        set { SetValue(UIScalerProperty, value); }
    }

    public static readonly DependencyProperty UIScalerProperty =
        DependencyProperty.Register(nameof(UIScaler), typeof(UIScaler), typeof(UIScalerBindingProxy), new UIPropertyMetadata(null));
}