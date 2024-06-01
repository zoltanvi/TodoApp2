using Modules.Common.ViewModel;

namespace TodoApp2.Core;

public class ShortcutsPageViewModel : BaseViewModel
{
    private readonly AppViewModel _appViewModel;

    public ShortcutsPageViewModel()
    {
    }

    public ShortcutsPageViewModel(AppViewModel applicationViewModel)
    {
        _appViewModel = applicationViewModel;
    }
}
