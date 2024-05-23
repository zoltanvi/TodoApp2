using System.Windows;

namespace TodoApp2;

public class MarginUpdater : BaseAttachedProperty<MarginUpdater, double>
{
    public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        double oldValue = (double)e.OldValue;
        double newValue = (double)e.NewValue;

        if (sender is FrameworkElement element && element.Margin.Left < 0)
        {
            // Removes the animation from margin which could have made the property to be unsettable
            element.BeginAnimation(FrameworkElement.MarginProperty, null);
            element.Margin = new Thickness(-newValue, 0, 0, 0);
        }
    }
}
