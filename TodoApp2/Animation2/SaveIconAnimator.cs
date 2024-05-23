using System;
using System.Windows;
using System.Windows.Threading;

namespace TodoApp2;

public static class SaveIconAnimator
{
    private static DispatcherTimer _timer;

    public static readonly DependencyProperty AnimateProperty =
        DependencyProperty.RegisterAttached("Animate", typeof(bool), typeof(SaveIconAnimator), new FrameworkPropertyMetadata(false, OnAnimateChanged));

    public static bool GetAnimate(FrameworkElement element)
    {
        return (bool)element.GetValue(AnimateProperty);
    }

    public static void SetAnimate(FrameworkElement element, bool value)
    {
        element.SetValue(AnimateProperty, value);
    }

    private static void OnAnimateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            InitializeTimer();

            _timer.Stop();
            _timer.Start();

            _timer.Tag = element;
            element.Opacity = 1;
        }
    }

    private static void InitializeTimer()
    {
        if (_timer == null)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }
    }

    private static void OnTimerTick(object sender, EventArgs e)
    {
        _timer.Stop();
        FrameworkElement element = (FrameworkElement)_timer.Tag;
        element.Opacity = 0;
    }
}
