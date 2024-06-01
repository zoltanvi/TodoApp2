using Modules.Common.ViewModel;

namespace TodoApp2;

/// <summary>
/// A base page with added ViewModel support
/// </summary>
public class BasePage<TViewModel> : BasePage
    where TViewModel : BaseViewModel, new()
{
    /// <summary>
    /// The View Model associated with this page
    /// </summary>
    private TViewModel _viewModel;

    /// <summary>
    /// The View Model associated with this page
    /// </summary>
    public TViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            // If nothing has changed, return
            if (_viewModel == value)
            {
                return;
            }

            // Update the value
            _viewModel = value;

            // Update the base property
            BaseViewModelObject = _viewModel;

            //Set the data context for this page
            DataContext = _viewModel;
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
