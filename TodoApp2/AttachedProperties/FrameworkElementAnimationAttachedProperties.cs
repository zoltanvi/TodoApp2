using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A base class to run any animation method when a boolean is set to true
    /// and a reversed animation when set to false
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public abstract class AnimateBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
        where Parent : BaseAttachedProperty<Parent, bool>, new()
    {

        #region Public Properties

        /// <summary>
        /// A flag indicating if this is the first time this property has been loaded
        /// </summary>
        protected bool FirstLoad { get; set; } = true;

        protected float BasicAnimationDuration => FirstLoad ? 0 : 0.3f;

        #endregion

        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Get the framework element
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            // Don't fire if the value doesn't change
            if (sender.GetValue(ValueProperty) == value && !FirstLoad)
            {
                return;
            }

            // On first load...
            if (FirstLoad)
            {
                // Create a single self-unhookable event
                // for the elements Loaded event
                RoutedEventHandler onLoaded = null;

                onLoaded = (o, args) =>
                {
                    // Unhook ourselves
                    element.Loaded -= onLoaded;

                    // No longer in first load
                    FirstLoad = false;

                    // Do desired animation
                    DoAnimation(element, (bool)value);
                };

                // Hook into the Loaded event of the element
                element.Loaded += onLoaded;
            }
            else
            {
                // Do desired animation
                DoAnimation(element, (bool)value);
            }
        }

        /// <summary>
        /// The animation method that is fired when the value chages
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        protected virtual void DoAnimation(FrameworkElement element, bool value)
        {
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the left on show
    /// and sliding out to the left on hide
    /// </summary>
    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
            {
                // Animate in
                await element.SlideAndFadeInFromLeftAsync(BasicAnimationDuration);
            }
            else
            {
                // Animate out
                await element.SlideAndFadeOutToLeftAsync(BasicAnimationDuration);
            }
        }
    }

    /// <summary>
    /// Animates a framework element sliding it in from the left on show
    /// and sliding out to the left on hide
    /// </summary>
    public class AnimateFadeInOutProperty : AnimateBaseProperty<AnimateFadeInOutProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
            {
                // Animate in
                await element.FadeInAsync(BasicAnimationDuration);
            }
            else
            {
                // Animate out
                await element.FadeOutAsync(BasicAnimationDuration);
                
            }
        }
    }
}
