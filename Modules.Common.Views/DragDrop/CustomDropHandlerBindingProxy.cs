using System.Windows;

namespace Modules.Common.Views.DragDrop;

public class CustomDropHandlerBindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new CustomDropHandlerBindingProxy();
    }

    public CustomDropHandler Handler
    {
        get { return (CustomDropHandler)GetValue(HandlerProperty); }
        set { SetValue(HandlerProperty, value); }
    }

    public static readonly DependencyProperty HandlerProperty =
        DependencyProperty.Register(nameof(Handler), typeof(CustomDropHandler), typeof(CustomDropHandlerBindingProxy), new UIPropertyMetadata(null));
}
