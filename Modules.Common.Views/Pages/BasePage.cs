using Modules.Common.Views.Animations;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Common.Views.Pages;

/// <summary>
/// The base page for all pages to gain base functionality
/// </summary>
public class BasePage : Page
{
    /// <summary>
    /// The View Model associated with this page
    /// </summary>
    protected object BaseViewModelObject;

    /// <summary>
    /// The animation to play when the page is loaded
    /// </summary>
    public PageAnimation PageLoadAnimation { get; set; } = PageAnimation.FadeIn;

    /// <summary>
    /// The animation to play when the page is unloaded
    /// </summary>
    public PageAnimation PageUnloadAnimation { get; set; } = PageAnimation.FadeOut;

    /// <summary>
    /// The time any load or unload animation takes to complete
    /// </summary>
    public float PageLoadAnimationDurationSeconds { get; set; } = 0.4f;

    /// <summary>
    /// A flag to indicate if this page should animate out on load.
    /// Useful for when we are moving the page to another frame
    /// </summary>
    public bool ShouldAnimateOut { get; set; }

    /// <summary>
    /// The View Model associated with this page
    /// </summary>
    public object ViewModelObject
    {
        get => BaseViewModelObject;
        set
        {
            // If nothing has changed, return
            if (BaseViewModelObject == value)
                return;

            // Update the value
            BaseViewModelObject = value;

            // Fire the view model changed method
            OnViewModelChanged();

            // Set the data context for this page
            DataContext = BaseViewModelObject;
        }
    }

    public BasePage()
    {
        // If we are animating in, hide to begin with
        if (PageLoadAnimation != PageAnimation.None)
        {
            Visibility = Visibility.Collapsed;
        }

        // Listen out for the page loading
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Once the page is loaded, perform any required animation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ShouldAnimateOut)
        {
            await AnimateOut();
        }
        else
        {
            await AnimateIn();
        }
    }

    /// <summary>
    /// Animates in this page
    /// </summary>
    /// <returns></returns>
    public async Task AnimateIn()
    {
        // Make sure we have something to do
        if (PageLoadAnimation == PageAnimation.None)
        {
            return;
        }

        switch (PageLoadAnimation)
        {
            case PageAnimation.FadeIn:
            {
                // Start the animation
                await this.FadeIn(PageLoadAnimationDurationSeconds);

                break;
            }
        }
    }

    /// <summary>
    /// Animates out this page
    /// </summary>
    /// <returns></returns>
    public async Task AnimateOut()
    {
        // Make sure we have something to do
        if (PageUnloadAnimation == PageAnimation.None)
        {
            return;
        }

        switch (PageLoadAnimation)
        {
            case PageAnimation.FadeOut:
            {
                // Start the animation
                await this.FadeOut(PageLoadAnimationDurationSeconds);

                break;
            }
        }
    }

    /// <summary>
    /// Fired when the view model changes
    /// </summary>
    protected virtual void OnViewModelChanged()
    {
    }
}