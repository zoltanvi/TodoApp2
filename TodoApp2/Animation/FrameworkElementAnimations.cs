using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Helpers to animate framework elements in specific ways
    /// </summary>
    public static class FrameworkElementAnimations
    {
        public static async Task SlideAndFadeInFromRightAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideInFromRight(seconds, element.ActualWidth, keepMargin: keepMargin);
                storyboard.AddFadeIn(seconds);

            }, seconds, keepMargin);
        }

        public static async Task SlideInFromLeftSideMenuAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideInFromLeft(seconds, IoC.UIScaler.SideMenuWidth, keepMargin: keepMargin);
                //storyboard.AddFadeIn(seconds);

            }, seconds, keepMargin);
        }

        public static async Task SlideOutToLeftSideMenuAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideOutToLeft(seconds, IoC.UIScaler.SideMenuWidth, keepMargin: keepMargin);
                //storyboard.AddFadeOut(seconds);

            }, seconds, keepMargin);
        }

        public static async Task SlideAndFadeOutToRightAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideOutToRight(seconds, element.ActualWidth, keepMargin: keepMargin);
                storyboard.AddFadeOut(seconds);

            }, seconds, keepMargin);
        }

        public static async Task SlideInFromBottomAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideFromBottom(seconds, element.ActualHeight);

            }, seconds);
        }

        public static async Task SlideOutToBottomAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideToBottom(seconds, element.ActualHeight);

            }, seconds);
        }

        public static async Task SlideInFromTopAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideFromTop(seconds, element.ActualHeight * 2);

            }, seconds);
        }

        public static async Task SlideOutToTopAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddSlideToTop(seconds, element.ActualHeight * 2);

            }, seconds);
        }

        public static async Task FadeInAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            //element.ShowElement();
            element.Visibility = Visibility.Visible;

            await DoAnimation(element, storyboard =>
            {
                storyboard.AddFadeIn(seconds);
            }, seconds);

            //element.Visibility = Visibility.Visible;
            //element.ShowElement();
        }

        public static async Task FadeOutAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            await DoAnimation(element, storyboard =>
            {
                storyboard.AddFadeOut(seconds);
            }, seconds);

            element.Visibility = Visibility.Collapsed;
            //element.HideElement();
        }

        /// <summary>
        /// Grows an element to full size
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static async Task GrowAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            const double fromScaleX = 0.0;
            const double fromScaleY = 0.0;
            const double toScaleX = 1.0;
            const double toScaleY = 1.0;
            const double centerX = 0.5;
            const double centerY = 0.5;

            // Create the storyboard
            Storyboard sb = new Storyboard();

            ScaleTransform scale = new ScaleTransform(fromScaleX, fromScaleX);

            // Set the center as pivot point
            element.RenderTransformOrigin = new Point(centerX, centerY);
            element.RenderTransform = scale;

            // Add fade in animation
            sb.AddScale(seconds, fromScaleX, fromScaleY, toScaleX, toScaleY);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Shrinks an element to zero size
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <returns></returns>
        public static async Task ShrinkAsync(this FrameworkElement element, float seconds = 0.3f)
        {
            const double fromScaleX = 1.0;
            const double fromScaleY = 1.0;
            const double toScaleX = 0.0;
            const double toScaleY = 0.0;
            const double centerX = 0.5;
            const double centerY = 0.5;

            // Create the storyboard
            Storyboard sb = new Storyboard();

            ScaleTransform scale = new ScaleTransform(fromScaleX, fromScaleY);

            // Set the center as pivot point
            element.RenderTransformOrigin = new Point(centerX, centerY);
            element.RenderTransform = scale;

            // Add scale animation
            sb.AddScale(seconds, fromScaleX, fromScaleY, toScaleX, toScaleY);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        private static async Task DoAnimation(this FrameworkElement element, Action<Storyboard> animationFunc, float seconds = 0.3f, bool keepMargin = true)
        {
            Storyboard sb = new Storyboard();

            animationFunc.Invoke(sb);

            sb.Begin(element);
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)(seconds * 1000));
            
            // Remove storyboard
            sb.Remove(element);
        }
    }
}