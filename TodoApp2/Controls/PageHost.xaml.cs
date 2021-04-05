using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for PageHost.xaml
    /// </summary>
    public partial class PageHost : UserControl
    {
        /// <summary>
        /// The current page to show in the page host
        /// </summary>
        public ApplicationPage CurrentPage
        {
            get => (ApplicationPage)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CurrentPage"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(ApplicationPage), typeof(PageHost), new UIPropertyMetadata(default(ApplicationPage), null, CurrentPagePropertyChanged));

        /// <summary>
        /// The current page to show in the page host
        /// </summary>
        public BaseViewModel CurrentPageViewModel
        {
            get => (BaseViewModel)GetValue(CurrentPageViewModelProperty);
            set => SetValue(CurrentPageViewModelProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CurrentPageViewModel"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageViewModelProperty =
            DependencyProperty.Register(nameof(CurrentPageViewModel), typeof(BaseViewModel), typeof(PageHost), new UIPropertyMetadata());

        public PageHost()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the <see cref="CurrentPage"/> value has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        private static object CurrentPagePropertyChanged(DependencyObject d, object value)
        {
            // Get current values
            ApplicationPage currentPage = (ApplicationPage)value;
            object currentPageViewModel = d.GetValue(CurrentPageViewModelProperty);

            // Get the frames
            Frame oldPageFrame = ((PageHost)d).OldPage;
            Frame newPageFrame = ((PageHost)d).NewPage;

            // Store the current page content as the old page
            object oldPageContent = newPageFrame.Content;

            // Remove current page from new page frame
            //newPageFrame.Content = null;

            // Move the previous page into the old page frame
            //oldPageFrame.Content = oldPageContent;

            // Animate out previous page when the Loaded event fires
            // right after this call due to moving frames
            if (oldPageContent is BasePage oldPage)
            {
                // Tell old page to animate out
                //oldPage.ShouldAnimateOut = true;

                //// Once it is done, remove it
                //Task.Delay((int)(oldPage.PageLoadAnimationDurationSeconds * 1000)).ContinueWith(t =>
                //{
                //    // Remove old page
                //    Application.Current.Dispatcher.Invoke(() =>
                //    {
                        if (oldPage.ViewModelObject is IDisposable disposableViewModel)
                        {
                            // Dispose the old ViewModel before switching to the new page
                            disposableViewModel.Dispose();
                        }

                        //oldPageFrame.Content = null;
                //    });
                //});
            }

            // Set the new page content
            newPageFrame.Content = currentPage.ToBasePage(currentPageViewModel);

            return value;
        }
    }
}