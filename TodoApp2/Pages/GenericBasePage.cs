using TodoApp2.Core;

namespace TodoApp2
{
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

                // Update the base property
                BaseViewModelObject = m_ViewModel;

                //Set the data context for this page
                DataContext = m_ViewModel;
            }
        }

        /// <summary>
        /// Constructor with specific view model
        /// </summary>
        /// <param name="specificViewModel">The specific view model to use, if any</param>
        public BasePage(TViewModel specificViewModel)
        {
            // Set specific view model
            ViewModel = specificViewModel;
        }
    }
}
