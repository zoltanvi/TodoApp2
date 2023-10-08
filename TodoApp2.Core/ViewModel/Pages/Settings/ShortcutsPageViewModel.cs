namespace TodoApp2.Core
{
    public class ShortcutsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel _AppViewModel;

        public ShortcutsPageViewModel()
        {
        }

        public ShortcutsPageViewModel(AppViewModel applicationViewModel)
        {
            _AppViewModel = applicationViewModel;
        }
    }
}
