using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace TodoApp2
{
    /// <summary>
    /// Animation helpers for <see cref="Storyboard"/>
    /// </summary>
    public static class StoryboardHelpers
    {
        private const string PropertyNameOpacity = "Opacity";
        private const string PropertyNameMargin = "Margin";

        /// <summary>
        /// Adds a fade in animation to the storyboard
        /// </summary>
        /// <param name="storyBoard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        public static void AddFadeIn(this Storyboard storyBoard, float seconds, float decelerationRatio = 0.9f)
        {
            // Create the opacity animate from 0 to 1
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 0.0,
                To = 1.0,
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameOpacity));
            
            // Add this to the storyboard
            storyBoard.Children.Add(animation);
        }


        /// <summary>
        /// Adds a fade out animation to the storyboard
        /// </summary>
        /// <param name="storyBoard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        public static void AddFadeOut(this Storyboard storyBoard, float seconds, float decelerationRatio = 0.9f)
        {
            // Create the opacity animate from 1 to 0
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 1.0,
                To = 0.0,
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameOpacity));

            // Add this to the storyboard
            storyBoard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide from right animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the right to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        public static void AddSlideFromRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(keepMargin ? offset : 0, 0, -offset, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide from left animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the left to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        public static void AddSlideFromLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide to left animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the left to end at</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        public static void AddSlideToLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide to right animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the right to end at</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        public static void AddSlideToRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(keepMargin ? offset : 0, 0, -offset, 0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath(PropertyNameMargin));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a grow animation to the storyboard
        /// </summary>
        /// <param name="storyBoard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="originalWidth">The element's original width</param>
        /// <param name="originalHeight">The element's original height</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        public static void AddGrow(this Storyboard storyBoard, float seconds, double originalWidth, double originalHeight, float decelerationRatio = 0.9f)
        {
            // Create the width animate from 0 to original size
            var widthAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 0.0,
                To = originalWidth,
                DecelerationRatio = decelerationRatio
            };

            // Create the height animate from 0 to original size
            var heightAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 0.0,
                To = originalHeight,
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));

            // Add this to the storyboard
            storyBoard.Children.Add(widthAnimation);
            storyBoard.Children.Add(heightAnimation);
        }

        /// <summary>
        /// Adds a shrink animation to the storyboard
        /// </summary>
        /// <param name="storyBoard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="originalWidth">The element's original width</param>
        /// <param name="originalHeight">The element's original height</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        public static void AddShrink(this Storyboard storyBoard, float seconds, double originalWidth, double originalHeight, float decelerationRatio = 0.9f)
        {
            // Create the width animate from 0 to original size
            var widthAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = originalWidth,
                To = 0.0,
                DecelerationRatio = decelerationRatio
            };

            // Create the height animate from 0 to original size
            var heightAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = originalHeight,
                To = 0.0,
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));

            // Add this to the storyboard
            storyBoard.Children.Add(widthAnimation);
            storyBoard.Children.Add(heightAnimation);
        }
    }
}
