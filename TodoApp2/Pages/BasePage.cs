using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TodoApp2.Animation;
using TodoApp2.Animations;
using TodoApp2.ViewModel;

namespace TodoApp2
{
    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage<VM> : Page
    where VM : BaseViewModel, new()
    {
        #region Private Members

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private VM m_ViewModel;

        #endregion

        #region Public properties

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
        /// The View Model associated with this page
        /// </summary>
        public VM ViewModel
        {
            get { return m_ViewModel; }
            set
            {
                // If nothing has changed, return
                if (m_ViewModel == value)
                {
                    return;
                }

                // Update the value
                m_ViewModel = value;

                //Set the data context for this page
                this.DataContext = m_ViewModel;
            }
        }

        #endregion

        public BasePage()
        {
            // If we are animating in, hide to begin with
            if (PageLoadAnimation != PageAnimation.None)
            {
                Visibility = Visibility.Collapsed;
            }

            // Listen out for the page loading
            this.Loaded += OnLoaded;

            // Create a default view model
            this.ViewModel = new VM();
        }

        /// <summary>
        /// Once the page is loaded, perform any required animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Animate the page in
            await AnimateIn();
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
    }
}
