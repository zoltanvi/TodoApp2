using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Modules.Common.Views.AttachedProperties;

/// <summary>
/// The NoFrameHistory attached property for creating a <see cref="Frame"/> that never shows navigation
/// and keeps the navigation history empty
/// </summary>
public class NoFrameHistory : BaseAttachedProperty<NoFrameHistory, bool>
{
    public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        // Get the frame
        if (!(sender is Frame frame))
        {
            return;
        }

        // Hide navigation bar
        frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

        frame.Navigated += (o, args) => ((Frame)o).NavigationService.RemoveBackEntry();
    }
}