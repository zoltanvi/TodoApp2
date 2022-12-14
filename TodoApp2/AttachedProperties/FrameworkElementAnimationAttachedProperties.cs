using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A base class to run any animation method when a boolean is set to true
    /// and a reversed animation when set to false
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    public abstract class AnimateBaseProperty<TParent> : BaseAttachedProperty<TParent, bool>
        where TParent : BaseAttachedProperty<TParent, bool>, new()
    {
        #region Protected Properties

        protected float DefaultAnimationDuration = 0.3f;
        protected float FastAnimationDuration = 0.15f;

        /// <summary>
        /// True if this is the very first time the value has been updated
        /// Used to make sure we run the logic at least once during first load
        /// </summary>
        protected readonly Dictionary<WeakReference, bool> AlreadyLoaded = new Dictionary<WeakReference, bool>();

        /// <summary>
        /// The most recent value used if we get a value changed before we do the first load
        /// </summary>
        protected readonly Dictionary<WeakReference, bool> FirstLoadValue = new Dictionary<WeakReference, bool>();

        #endregion Protected Properties

        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Get the framework element
            if (!(sender is FrameworkElement element))
                return;

            // Try and get the already loaded reference
            var alreadyLoadedReference = AlreadyLoaded.FirstOrDefault(f => Equals(f.Key.Target, sender));

            // Try and get the first load reference
            var firstLoadReference = FirstLoadValue.FirstOrDefault(f => Equals(f.Key.Target, sender));

            // Don't fire if the value doesn't change
            if ((bool)sender.GetValue(ValueProperty) == (bool)value && alreadyLoadedReference.Key != null)
                return;

            // On first load...
            if (alreadyLoadedReference.Key == null)
            {
                // Create weak reference
                var weakReference = new WeakReference(sender);

                // Flag that we are in first load but have not finished it
                AlreadyLoaded[weakReference] = false;

                // Start off hidden before we decide how to animate
                element.Visibility = Visibility.Hidden;

                // Create a single self-unhookable event
                // for the elements Loaded event
                async void OnLoaded(object ss, RoutedEventArgs ee)
                {
                    // Unhook ourselves
                    element.Loaded -= OnLoaded;

                    // Slight delay after load is needed for some elements to get laid out
                    // and their width/heights correctly calculated
                    await Task.Delay(5);

                    // Refresh the first load value in case it changed
                    // since the 5ms delay
                    firstLoadReference = FirstLoadValue.FirstOrDefault(f => Equals(f.Key.Target, sender));

                    // Do desired animation
                    DoAnimation(element, firstLoadReference.Key != null ? firstLoadReference.Value : (bool)value, true);

                    // Flag that we have finished first load
                    AlreadyLoaded[weakReference] = true;
                }

                // Hook into the Loaded event of the element
                element.Loaded += OnLoaded;
            }
            // If we have started a first load but not fired the animation yet, update the property
            else if (alreadyLoadedReference.Value == false)
            {
                FirstLoadValue[new WeakReference(sender)] = (bool)value;
            }
            else
            {
                // Do desired animation
                DoAnimation(element, (bool)value, false);
            }
        }

        /// <summary>
        /// The animation method that is fired when the value changes
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="value">The new value</param>
        /// <param name="firstLoad">Does the element just had it's first load</param>
        protected virtual void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the left on show
    /// and sliding out to the left on hide
    /// </summary>
    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideAndFadeInFromLeftAsync(firstLoad ? 0 : 0.06f);
            else
                // Animate out
                await element.SlideAndFadeOutToLeftAsync(firstLoad ? 0 : DefaultAnimationDuration);
        }
    }

    public class AnimateSlideInFromBottomProperty : AnimateBaseProperty<AnimateSlideInFromBottomProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideInFromBottomAsync(firstLoad ? 0 : 1);
            else
                // Animate out
                await element.SlideOutToBottomAsync(firstLoad ? 0 : 0.5f);
        }
    }

    public class AnimateSlideInFromTopProperty : AnimateBaseProperty<AnimateSlideInFromTopProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideInFromTopAsync(firstLoad ? 0 : 1);
            else
                // Animate out
                await element.SlideOutToTopAsync(firstLoad ? 0 : 0.5f);
        }
    }

    public class AnimateFadeInProperty : AnimateBaseProperty<AnimateFadeInProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.FadeInAsync(firstLoad ? 0 : DefaultAnimationDuration);
            else
                // Animate out
                await element.FadeOutAsync(firstLoad ? 0 : DefaultAnimationDuration);
        }
    }

    public class AnimateFadeInAndOutProperty : AnimateBaseProperty<AnimateFadeInAndOutProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (!firstLoad)
            {
                await element.FadeInAsync(DefaultAnimationDuration);
                await Task.Delay(1000);
                await element.FadeOutAsync(DefaultAnimationDuration);
            }
        }
    }

    public class AnimateGrowProperty : AnimateBaseProperty<AnimateGrowProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.GrowAsync(firstLoad ? 0 : FastAnimationDuration);
            else
                // Animate out
                await element.ShrinkAsync(firstLoad ? 0 : FastAnimationDuration);
        }
    }
}