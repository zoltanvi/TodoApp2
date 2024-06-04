using System.Windows;
using System.Windows.Media.Animation;

namespace Modules.Common.Views.Animations;

/// <summary>
/// Animation extension methods for <see cref="Storyboard"/>
/// </summary>
public static class StoryboardExtensions
{
    private const string PropertyNameOpacity = "Opacity";
    private const string PropertyNameMargin = "Margin";

    public static void ResetStoryboard(this Storyboard storyboard)
    {
        storyboard.Children.Clear();
    }

    public static void AddFadeIn(this Storyboard storyBoard, float seconds, float decelerationRatio = 0.9f, double from = 0.0, double to = 1.0)
    {
        var animation = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = from,
            To = to,
            DecelerationRatio = decelerationRatio
        };

        Timeline.SetDesiredFrameRate(animation, 60); // 60 FPS
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameOpacity));
        storyBoard.Children.Add(animation);
    }

    public static void AddFadeOut(this Storyboard storyBoard, float seconds, float decelerationRatio = 0.9f, double from = 1.0, double to = 0.0)
    {
        var animation = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = from,
            To = to,
            DecelerationRatio = decelerationRatio
        };

        Timeline.SetDesiredFrameRate(animation, 60); // 60 FPS
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameOpacity));
        storyBoard.Children.Add(animation);
    }

    public static void AddSlideInFromRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
    {
        var animation = new ThicknessAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = new Thickness(keepMargin ? offset : 0, 0, -offset, 0),
            To = new Thickness(0),
            DecelerationRatio = decelerationRatio,
            EasingFunction = new QuadraticEase()
        };

        Timeline.SetDesiredFrameRate(animation, 60); // 60 FPS
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));
        storyboard.Children.Add(animation);
    }

    public static void AddSlideInFromLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
    {
        var animation = new ThicknessAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
            To = new Thickness(0),
            DecelerationRatio = decelerationRatio,
            EasingFunction = new QuadraticEase()
        };

        Timeline.SetDesiredFrameRate(animation, 60); // 60 FPS
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));
        storyboard.Children.Add(animation);
    }

    public static void AddSlideOutToLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
    {
        storyboard.AddSlide(seconds, decelerationRatio, new Thickness(0), new Thickness(-offset, 0, keepMargin ? offset : 0, 0));
    }

    public static void AddSlideOutToRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
    {
        storyboard.AddSlide(seconds, decelerationRatio, new Thickness(0), new Thickness(keepMargin ? offset : 0, 0, -offset, 0));
    }

    public static void AddSlideFromBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
    {
        storyboard.AddSlide(seconds, decelerationRatio, new Thickness(0, 0, 0, -offset - 10), new Thickness(0),
            new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 });
    }

    public static void AddSlideToBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
    {
        storyboard.AddSlide(seconds, decelerationRatio, new Thickness(0), new Thickness(0, 0, 0, -offset - 10),
            new BackEase { EasingMode = EasingMode.EaseIn, Amplitude = 0.3 });
    }

    public static void AddSlideFromTop(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
    {
        storyboard.AddSlide(seconds, decelerationRatio, new Thickness(0, -offset - 10, 0, 0), new Thickness(0),
            new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 });
    }

    public static void AddSlideToTop(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
    {
        storyboard.AddSlide(seconds, decelerationRatio, new Thickness(0), new Thickness(0, -offset - 10, 0, 0),
            new BackEase { EasingMode = EasingMode.EaseIn, Amplitude = 0.3 });
    }

    private static void AddSlide(this Storyboard storyboard, float seconds, float decelerationRatio, Thickness from, Thickness to, IEasingFunction easingFunction = null)
    {
        var animation = new ThicknessAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = from,
            To = to,
            DecelerationRatio = decelerationRatio,
            EasingFunction = easingFunction
        };

        Timeline.SetDesiredFrameRate(animation, 60); // 60 FPS
        Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));
        storyboard.Children.Add(animation);
    }

    public static void AddScale(this Storyboard storyBoard, float seconds, double fromScaleX, double fromScaleY, double toScaleX, double toScaleY)
    {
        var shrinkXAnimation = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = fromScaleX,
            To = toScaleX,
        };

        var shrinkYAnimation = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            From = fromScaleY,
            To = toScaleY,
        };

        Timeline.SetDesiredFrameRate(shrinkXAnimation, 60); // 60 FPS
        Timeline.SetDesiredFrameRate(shrinkYAnimation, 60); // 60 FPS
        Storyboard.SetTargetProperty(shrinkXAnimation, new PropertyPath("RenderTransform.ScaleX"));
        Storyboard.SetTargetProperty(shrinkYAnimation, new PropertyPath("RenderTransform.ScaleY"));

        storyBoard.Children.Add(shrinkXAnimation);
        storyBoard.Children.Add(shrinkYAnimation);
    }

    public static void HideElement(this FrameworkElement element)
    {
        element.Margin = new Thickness(-element.ActualWidth, 0, element.ActualWidth, 0);
    }

    public static void ShowElement(this FrameworkElement element)
    {
        element.Margin = new Thickness(0);
    }
}