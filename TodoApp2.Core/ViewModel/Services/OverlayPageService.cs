using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using System;
using System.Windows.Input;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

public class OverlayPageService : BaseViewModel, IOverlayPageService
{
    private readonly AppViewModel _appViewModel;
    private readonly IAppContext _context;

    /// <summary>
    /// True if the overlay background should be shown
    /// </summary>
    public bool OverlayBackgroundVisible { get; set; }

    public TaskViewModel Task { get; set; }

    /// <summary>
    /// A page content dependent command that executes when the overlay background is clicked.
    /// </summary>
    public ICommand BackgroundClickedCommand { get; private set; }

    public OverlayPageService(IAppContext context, AppViewModel appViewModel)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(appViewModel);

        _context = context;
        _appViewModel = appViewModel;
    }

    public void SetBackgroundClickedAction(Action action)
    {
        BackgroundClickedCommand = new RelayCommand(action);
    }

    public void CloseSideMenu()
    {
        _appViewModel.SideMenuVisible = false;
        OverlayBackgroundVisible = false;
    }

    public void ClosePage()
    {
        if (_appViewModel.OverlayPageVisible)
        {
            _appViewModel.OverlayPageVisible = false;
            OverlayBackgroundVisible = false;

            _appViewModel.CloseOverlayPage();

            BackgroundClickedCommand?.Execute(null);
        }
    }
}