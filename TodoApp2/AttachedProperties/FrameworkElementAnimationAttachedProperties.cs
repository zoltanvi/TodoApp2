using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TodoApp2
{
    /// <summary>
    /// Animates a framework element sliding it in from the left on show
    /// and sliding out to the left on hide
    /// </summary>
    public class AnimateSlideInFromLeftSideMenuProperty : AnimateBaseProperty<AnimateSlideInFromLeftSideMenuProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value, bool firstLoad)
        {
            if (value)
                // Animate in
                await element.SlideInFromLeftSideMenuAsync(firstLoad ? 0 : 0.06f, false);
            else
                // Animate out
                await element.SlideOutToLeftSideMenuAsync(firstLoad ? 0 : DefaultAnimationDuration, false);
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