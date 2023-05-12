using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        public object TitleBarBgBrush
        {
            get => _TitleBarBgBrush;
            set
            {
                _TitleBarBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(TitleBarBgBrush), _TitleBarBgBrush);
            }
        }

        public object PageBgBrush
        {
            get => _PageBgBrush;
            set
            {
                _PageBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(PageBgBrush), _PageBgBrush);
            }
        }

        public object SideMenuBgBrush
        {
            get => _SideMenuBgBrush;
            set
            {
                _SideMenuBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(SideMenuBgBrush), _SideMenuBgBrush);
            }
        }

        public object BottomBarBgBrush
        {
            get => _BottomBarBgBrush;
            set
            {
                _BottomBarBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(BottomBarBgBrush), _BottomBarBgBrush);
            }
        }

        public object ForegroundBrush
        {
            get => _ForegroundBrush;
            set
            {
                _ForegroundBrush = value;
                _resourceUpdater.UpdateResource(nameof(ForegroundBrush), _ForegroundBrush);
            }
        }

        public object TaskBgBrush
        {
            get => _TaskBgBrush;
            set
            {
                _TaskBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(TaskBgBrush), _TaskBgBrush);
            }
        }

        public object SelectedCategoryBgBrush
        {
            get => _SelectedCategoryBgBrush;
            set
            {
                _SelectedCategoryBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(SelectedCategoryBgBrush), _SelectedCategoryBgBrush);
            }
        }

        public object HoverCategoryBgBrush
        {
            get => _HoverCategoryBgBrush;
            set
            {
                _HoverCategoryBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(HoverCategoryBgBrush), _HoverCategoryBgBrush);
            }
        }

        private readonly IResourceUpdater _resourceUpdater;
        private readonly AppViewModel _AppViewModel;
        private object _TitleBarBgBrush;
        private object _PageBgBrush;
        private object _SideMenuBgBrush;
        private object _BottomBarBgBrush;
        private object _ForegroundBrush;
        private object _TaskBgBrush;
        private object _SelectedCategoryBgBrush;
        private object _HoverCategoryBgBrush;

        public ICommand ChangeThemeCommand { get; }
        public ICommand GoBackCommand { get; }

        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            _AppViewModel = applicationViewModel;
            _resourceUpdater = IoC.ResourceUpdater;
            ChangeThemeCommand = new RelayParameterizedCommand(ChangeTheme);
            GoBackCommand = new RelayCommand(() => { _AppViewModel.UpdateMainPage(); });

            UpdateBrushes();
        }

        private void UpdateBrushes()
        {
            _TitleBarBgBrush = _resourceUpdater.GetResourceValue(nameof(TitleBarBgBrush));
            _PageBgBrush = _resourceUpdater.GetResourceValue(nameof(PageBgBrush));
            _SideMenuBgBrush = _resourceUpdater.GetResourceValue(nameof(SideMenuBgBrush));
            _BottomBarBgBrush = _resourceUpdater.GetResourceValue(nameof(BottomBarBgBrush));
            _ForegroundBrush = _resourceUpdater.GetResourceValue(nameof(ForegroundBrush));
            _TaskBgBrush = _resourceUpdater.GetResourceValue(nameof(TaskBgBrush));
            _SelectedCategoryBgBrush = _resourceUpdater.GetResourceValue(nameof(SelectedCategoryBgBrush));
            _HoverCategoryBgBrush = _resourceUpdater.GetResourceValue(nameof(HoverCategoryBgBrush));
        }

        private void ChangeTheme(object obj)
        {
            if (obj is ThemeViewModel themeViewModel)
            {
                _AppViewModel.ApplicationSettings.ActiveTheme = themeViewModel.Theme;

                UpdateBrushes();
            }
        }
    }
}
