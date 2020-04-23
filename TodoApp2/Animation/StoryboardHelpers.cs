using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace TodoApp2.Animations
{
    /// <summary>
    /// Animation helpers for <see cref="Storyboard"/>
    /// </summary>
    public static class StoryboardHelpers
    {
        private const string PropertyNameOpacity = "Opacity";

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
    }
}
