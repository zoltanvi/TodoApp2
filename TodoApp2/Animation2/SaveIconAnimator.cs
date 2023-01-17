using System;
using System.Windows;
using System.Windows.Threading;

namespace TodoApp2
{
    public static class SaveIconAnimator
    {
        private static DispatcherTimer m_Timer;

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

                m_Timer.Stop();
                m_Timer.Start();

                m_Timer.Tag = element;
                element.Opacity = 1;
            }
        }

        private static void InitializeTimer()
        {
            if (m_Timer == null)
            {
                m_Timer = new DispatcherTimer();
                m_Timer.Interval = TimeSpan.FromSeconds(1);
                m_Timer.Tick += OnTimerTick;
            }
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            m_Timer.Stop();
            FrameworkElement element = (FrameworkElement)m_Timer.Tag;
            element.Opacity = 0;
        }
    }
}
