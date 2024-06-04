using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Modules.Common.Views.Animations;

/// <summary>
/// Helpers to animate pages in specific ways
/// </summary>
public static class PageAnimations
{
    public static async Task FadeIn(this Page page, float seconds)
    {
        // Create the storyboard
        var sb = new Storyboard();

        // Add fade in animation
        sb.AddFadeIn(seconds);

        // Start animating
        sb.Begin(page);

        // Make page visible
        page.Visibility = Visibility.Visible;

        // Wait for it to finish
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }

    public static async Task FadeOut(this Page page, float seconds)
    {
        // Create the storyboard
        var sb = new Storyboard();

        // Add fade out animation
        sb.AddFadeOut(seconds);

        // Start animating
        sb.Begin(page);

        // Wait for it to finish
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
}