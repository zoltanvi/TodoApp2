using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// The base page for all pages to gain base functionality
    /// </summary>
    public class BasePage : Page
    {
        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private object m_ViewModel;

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
        public object ViewModelObject
        {
            get => m_ViewModel;
            set
            {
                // If nothing has changed, return
                if (m_ViewModel == value)
                    return;

                // Update the value
                m_ViewModel = value;

                // Fire the view model changed method
                OnViewModelChanged();

                // Set the data context for this page
                DataContext = m_ViewModel;
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

        /// <summary>
        /// Fired when the view model changes
        /// </summary>
        protected virtual void OnViewModelChanged()
        {

        }
    }


    /// <summary>
    /// A base page with added ViewModel support
    /// </summary>
    public class BasePage<TViewModel> : BasePage
        where TViewModel : BaseViewModel, new()
    {
        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private TViewModel m_ViewModel;

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public TViewModel ViewModel
        {
            get => m_ViewModel;
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
                DataContext = m_ViewModel;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage()
        {
            // Create a default view model
            ViewModel = IoC.Get<TViewModel>();
        }

        /// <summary>
        /// Constructor with specific view model
        /// </summary>
        /// <param name="specificViewModel">The specific view model to use, if any</param>
        public BasePage(TViewModel specificViewModel = null)
        {
            // Set specific view model
            if (specificViewModel != null)
            {
                ViewModel = specificViewModel;
            }
            else
            {
                // Create a default view model
                ViewModel = IoC.Get<TViewModel>();
            }

        }
    }
}
